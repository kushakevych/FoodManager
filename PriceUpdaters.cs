using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Playwright;

namespace FoodManager
{
    public static class PriceUpdaters
    {
        public static readonly string StoreATB = "АТБ";
        public static readonly string StoreSilpo = "Сільпо";
        public static readonly string StoreStolich = "Столичний ринок";
        public static readonly string StoreMetro = "Метро";
        public static readonly string StoreVK = "Велика Кишеня";
        public static readonly string StoreAshan = "Ашан";
        public static readonly string StoreNovus = "NOVUS";

        // --------- Simple shared resources ---------
        // Reduced HttpClient timeout to fail faster on slow/unresponsive endpoints
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        // Playwright single-browser pool
        private static IPlaywright? _pw;
        private static IBrowser? _browser;
        private static IBrowserContext? _context;
        private static SemaphoreSlim? _pageLimiter;
        private static readonly object _initLock = new object();
        private static bool _pwInitialized = false;

        // Stolichny JSON urls + in-memory cache (small)
        private static readonly string[] StolichJsonUrls = {
            "https://kyivopt.com/assets/cache_google/data_cache_fish_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_fruits_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_grocery_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_meat_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_milk_0.json"
        };
        private static List<StolichEntry>? _stolichCache;
        private static DateTime _stolichCachedAt = DateTime.MinValue;
        private static readonly TimeSpan StolichCacheTtl = TimeSpan.FromMinutes(60);
        private static readonly object _stolLock = new object();

        // --------- Lifecycle helpers (very small) ---------
        // Call once before many Playwright calls (choose parallelism 2-6)
        public static async Task EnsurePlaywrightAsync(int maxParallel = 4)
        {
            if (_pwInitialized) return;
            lock (_initLock)
            {
                if (_pwInitialized) return;
                _pwInitialized = true;
            }

            _pw = await Playwright.CreateAsync().ConfigureAwait(false);
            // tune args to reduce sandboxing / resource overhead; safe for many environments
            _browser = await _pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--disable-dev-shm-usage", "--disable-extensions", "--disable-gpu", "--no-sandbox", "--disable-background-networking" }
            }).ConfigureAwait(false);

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                Locale = "uk-UA",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
            }).ConfigureAwait(false);

            _pageLimiter = new SemaphoreSlim(Math.Max(1, maxParallel), Math.Max(1, maxParallel));
        }

        public static async Task DisposePlaywrightAsync()
        {
            try { if (_context != null) await _context.CloseAsync().ConfigureAwait(false); } catch { }
            try { if (_browser != null) await _browser.CloseAsync().ConfigureAwait(false); } catch { }
            try { _pw?.Dispose(); } catch { }
            _context = null; _browser = null; _pw = null;
            _pageLimiter?.Dispose(); _pageLimiter = null;
            _pwInitialized = false;
        }

        // Small helper to run with a page, reusing browser/context and limiting concurrency
        private static async Task<T> RunWithPageAsync<T>(Func<IPage, Task<T>> action, CancellationToken ct)
        {
            if (_context == null || _pageLimiter == null) throw new InvalidOperationException("Playwright not initialized. Call EnsurePlaywrightAsync first.");
            await _pageLimiter.WaitAsync(ct).ConfigureAwait(false);
            IPage page = null!;
            try
            {
                page = await _context.NewPageAsync().ConfigureAwait(false);

                // block heavy resources to speed up loads
                await page.RouteAsync("**/*", async route =>
                {
                    try
                    {
                        var t = route.Request.ResourceType;
                        if (t == "image" || t == "font" || t == "media")
                            await route.AbortAsync();
                        else
                            await route.ContinueAsync();
                    }
                    catch
                    {
                        try { await route.ContinueAsync(); } catch { }
                    }
                }).ConfigureAwait(false);

                var res = await action(page).ConfigureAwait(false);
                try { await page.CloseAsync().ConfigureAwait(false); } catch { }
                return res;
            }
            finally
            {
                try { if (page != null) await page.CloseAsync().ConfigureAwait(false); } catch { }
                _pageLimiter.Release();
            }
        }

        // --------- High-level updater used by UI ---------
        // returns true if p.Prices changed
        public static async Task<bool> UpdatePricesForProductAsync(Product p, CancellationToken ct)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            var changed = false;
            var links = p.Links ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // If a store is in ManualPriceSources, skip updating it
            var manual = p.ManualPriceSources ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (manual.Contains(StoreATB) == false && links.TryGetValue(StoreATB, out var atbUrl) && !string.IsNullOrWhiteSpace(atbUrl))
            {
                var price = await FetchPriceFromAtbAsync(atbUrl, ct).ConfigureAwait(false);
                if (price.HasValue) changed |= UpsertPrice(p, StoreATB, price.Value);
            }

            if (manual.Contains(StoreSilpo) == false && links.TryGetValue(StoreSilpo, out var silpoUrl) && !string.IsNullOrWhiteSpace(silpoUrl))
            {
                var price = await FetchPriceFromSilpoAsync(silpoUrl, ct).ConfigureAwait(false);
                if (price.HasValue) changed |= UpsertPrice(p, StoreSilpo, price.Value);
            }

            if (manual.Contains(StoreStolich) == false && links.TryGetValue(StoreStolich, out var stolQuery) && !string.IsNullOrWhiteSpace(stolQuery))
            {
                var price = await FetchPriceFromStolichnyRynokAsync(stolQuery, ct).ConfigureAwait(false);
                if (price.HasValue) changed |= UpsertPrice(p, StoreStolich, price.Value);
            }

            if (manual.Contains(StoreMetro) == false && links.TryGetValue(StoreMetro, out var metroUrl) && !string.IsNullOrWhiteSpace(metroUrl))
            {
                var price = await FetchPriceFromMetroAsync(metroUrl, ct).ConfigureAwait(false);
                if (price.HasValue) changed |= UpsertPrice(p, StoreMetro, price.Value);
            }

            if (manual.Contains(StoreVK) == false && links.TryGetValue(StoreVK, out var vkUrl) && !string.IsNullOrWhiteSpace(vkUrl))
            {
                // stub: keep for future
                var price = await FetchPriceFromVelykaKyshenyaAsync(vkUrl, ct).ConfigureAwait(false);
                if (price.HasValue) changed |= UpsertPrice(p, StoreVK, price.Value);
            }

            return changed;
        }

        private static bool UpsertPrice(Product p, string store, decimal price)
        {
            if (p.Prices == null) p.Prices = new Dictionary<string, decimal>();
            if (!p.Prices.TryGetValue(store, out var old) || old != price)
            {
                p.Prices[store] = price;
                return true;
            }
            return false;
        }

        // --------- ATB & Silpo via Playwright (compact) ---------
        // Reduced navigation timeout to fail faster (20s)
        private const int NAV_TIMEOUT_MS = 20000;

        public static async Task<decimal?> FetchPriceFromAtbAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            // per-call budget to fail fast if caller doesn't cancel
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linked.CancelAfter(TimeSpan.FromSeconds(8));
            var token = linked.Token;

            try
            {
                return await RunWithPageAsync(async page =>
                {
                    token.ThrowIfCancellationRequested();
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        // Try to navigate (shorter timeout than NAV_TIMEOUT_MS)
                        await page.GotoAsync(url, new PageGotoOptions
                        {
                            WaitUntil = WaitUntilState.DOMContentLoaded,
                            Timeout = Math.Min(NAV_TIMEOUT_MS, 8000)
                        });

                        // locate price container
                        var locator = page.Locator(".product-price__top").First;

                        // Wait specifically for the price element to appear (short wait)
                        try
                        {
                            await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = 3000 });
                        }
                        catch (TimeoutException)
                        {
                            // element not found quickly — try alternative selectors before giving up
                            var alt = page.Locator(".product-price, .price, [data-testid=\"product-price\"]");
                            try
                            {
                                await alt.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 2000 });
                                locator = alt.First;
                            }
                            catch { /* not found */ }
                        }

                        // 1) Try aria-label or combined parts (int + coin)
                        string? raw = null;
                        try
                        {
                            raw = await TryGetAttrOrText(locator, "aria-label").ConfigureAwait(false);
                        }
                        catch { raw = null; }

                        if (string.IsNullOrWhiteSpace(raw))
                        {
                            // Try to read integer part and coin part separately
                            string? intPart = null;
                            try
                            {
                                // outer span usually contains integer part (may include trailing punctuation)
                                intPart = (await locator.Locator("span").First.InnerTextAsync().ConfigureAwait(false))?.Trim();
                            }
                            catch { intPart = null; }

                            string? coinPart = null;
                            try
                            {
                                var coinLoc = locator.Locator(".product-price__coin");
                                if (await coinLoc.CountAsync().ConfigureAwait(false) > 0)
                                    coinPart = (await coinLoc.First.InnerTextAsync().ConfigureAwait(false))?.Trim();
                            }
                            catch { coinPart = null; }

                            if (!string.IsNullOrWhiteSpace(intPart) && !string.IsNullOrWhiteSpace(coinPart))
                            {
                                var intOnly = Regex.Match(intPart ?? "", @"\d+").Value;
                                raw = $"{intOnly}.{coinPart}";
                            }
                            else
                            {
                                // fallback: full innerText of container
                                try { raw = (await locator.InnerTextAsync().ConfigureAwait(false))?.Trim(); } catch { raw = null; }
                            }
                        }

                        sw.Stop();
                        try { DebugLogger.Log($"ATB fetch: {url} elapsed={sw.Elapsed.TotalSeconds:F2}s rawLen={(raw?.Length ?? 0)} raw='{raw}'"); } catch { }

                        if (string.IsNullOrWhiteSpace(raw)) return (decimal?)null;

                        // Normalize and extract number
                        // allow formats like "31.50", "31,50", "31 50" etc.
                        var normalized = Regex.Match(raw.Replace("\u00A0", " "), @"\d+(?:[.,]\d{2})?").Value;
                        if (string.IsNullOrWhiteSpace(normalized))
                        {
                            var m = Regex.Match(raw, @"(\d+)[^\d]+(\d{2})");
                            if (!m.Success) return (decimal?)null;
                            normalized = $"{m.Groups[1].Value}.{m.Groups[2].Value}";
                        }
                        normalized = normalized.Replace(',', '.').Trim();
                        if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                            return (decimal?)v;
                        return (decimal?)null;
                    }
                    catch (TimeoutException tex)
                    {
                        try { DebugLogger.LogError($"ATB navigation/selector timeout for {url}", tex); } catch { }
                        return null;
                    }
                }, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                try { DebugLogger.LogError($"ATB fetch failed for {url}", ex); } catch { }
                return null;
            }
        }

        public static async Task<decimal?> FetchPriceFromSilpoAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linked.CancelAfter(TimeSpan.FromSeconds(8));
            var token = linked.Token;

            try
            {
                return await RunWithPageAsync(async page =>
                {
                    token.ThrowIfCancellationRequested();
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = Math.Min(NAV_TIMEOUT_MS, 8000) });

                        var locator = page.Locator(".main-price.ng-star-inserted").First;
                        try
                        {
                            await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = 3000 });
                        }
                        catch (TimeoutException)
                        {
                            var alt = page.Locator(".product-price, .price, [data-testid=\"product-price\"]");
                            try
                            {
                                await alt.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 2000 });
                                locator = alt.First;
                            }
                            catch { /* not found */ }
                        }

                        string? raw = null;
                        try { raw = await TryGetAttrOrText(locator, "aria-label").ConfigureAwait(false); } catch { raw = null; }
                        if (string.IsNullOrWhiteSpace(raw))
                        {
                            try { raw = (await locator.InnerTextAsync().ConfigureAwait(false))?.Trim(); } catch { raw = null; }
                        }

                        sw.Stop();
                        try { DebugLogger.Log($"Silpo fetch: {url} elapsed={sw.Elapsed.TotalSeconds:F2}s rawLen={(raw?.Length ?? 0)} raw='{raw}'"); } catch { }

                        if (string.IsNullOrWhiteSpace(raw)) return (decimal?)null;
                        var m = Regex.Match(raw, @"(\d+(?:[.,]\d+)?)");
                        if (!m.Success) return (decimal?)null;
                        var s = m.Groups[1].Value.Replace(',', '.');
                        return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? (decimal?)v : null;
                    }
                    catch (TimeoutException tex)
                    {
                        try { DebugLogger.LogError($"Silpo navigation/selector timeout for {url}", tex); } catch { }
                        return null;
                    }
                }, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                try { DebugLogger.LogError($"Silpo fetch failed for {url}", ex); } catch { }
                return null;
            }
        }

        // small helpers for playwright locators
        private static async Task<string?> TryGetAttrOrText(ILocator loc, string attr)
        {
            try
            {
                var a = await loc.GetAttributeAsync(attr).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(a)) return a;
            }
            catch { }
            try { return await loc.InnerTextAsync().ConfigureAwait(false); } catch { return null; }
        }
        private static async Task<string?> TryInnerTextSafe(ILocator loc)
        {
            try { return await loc.InnerTextAsync().ConfigureAwait(false); } catch { return null; }
        }

        // --------- Metro: HttpClient + JSON-LD parse (compact) ---------
        public static async Task<decimal?> FetchPriceFromMetroAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseContentRead, ct).ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();
                var html = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(html)) return null;

                // find <script type="application/ld+json"> blocks
                var scriptPattern = @"<script[^>]*type\s*=\s*[""']application\/ld\+json[""'][^>]*>(.*?)<\/script>";
                var matches = Regex.Matches(html, scriptPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (Match m in matches)
                {
                    var json = m.Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(json)) continue;
                    try
                    {
                        using var doc = JsonDocument.Parse(json);
                        var price = ExtractOffersPrice(doc.RootElement);
                        if (price.HasValue) return price;
                    }
                    catch
                    {
                        // try split concatenated JSON
                        var parts = TrySplitJson(json);
                        foreach (var pJson in parts)
                        {
                            try
                            {
                                using var doc2 = JsonDocument.Parse(pJson);
                                var price2 = ExtractOffersPrice(doc2.RootElement);
                                if (price2.HasValue) return price2;
                            }
                            catch { }
                        }
                    }
                }

                // fallback regex: "offers": {... "price": "169.00"
                var offersPattern = @"""offers""\s*:\s*\{[^}]*?""price""\s*:\s*[""']?([0-9]+(?:[.,][0-9]+))[""']?";
                var ofm = Regex.Match(html, offersPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (ofm.Success)
                {
                    var s = ofm.Groups[1].Value.Replace(',', '.');
                    if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v)) return v;
                }

                return null;
            }
            catch (OperationCanceledException) { throw; }
            catch { return null; }
        }

        private static decimal? ExtractOffersPrice(JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Object)
            {
                if (el.TryGetProperty("offers", out var offers))
                {
                    if (offers.ValueKind == JsonValueKind.Object)
                    {
                        if (TryPriceFromElement(offers, out var p)) return p;
                    }
                    else if (offers.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var it in offers.EnumerateArray())
                        {
                            if (TryPriceFromElement(it, out var p2)) return p2;
                        }
                    }
                }
                // search deeper
                foreach (var prop in el.EnumerateObject())
                {
                    var res = ExtractOffersPrice(prop.Value);
                    if (res.HasValue) return res;
                }
            }
            else if (el.ValueKind == JsonValueKind.Array)
            {
                foreach (var it in el.EnumerateArray())
                {
                    var res = ExtractOffersPrice(it);
                    if (res.HasValue) return res;
                }
            }
            return null;
        }

        private static bool TryPriceFromElement(JsonElement e, out decimal? price)
        {
            price = null;
            if (e.ValueKind != JsonValueKind.Object) return false;
            if (e.TryGetProperty("offers", out var offers))
            {
                // nested offers
                return TryPriceFromElement(offers, out price);
            }
            if (e.TryGetProperty("price", out var pe))
            {
                if (pe.ValueKind == JsonValueKind.String)
                {
                    var s = pe.GetString() ?? "";
                    s = s.Replace(',', '.').Trim();
                    if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                    {
                        price = v; return true;
                    }
                }
                else if (pe.ValueKind == JsonValueKind.Number)
                {
                    if (pe.TryGetDecimal(out var v2)) { price = v2; return true; }
                }
            }
            return false;
        }

        private static string[] TrySplitJson(string json)
        {
            if (json.Contains("}{"))
            {
                var parts = json.Split(new[] { "}\n{", "}{", "}\r\n{" }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => { var t = p.Trim(); if (!t.StartsWith("{")) t = "{" + t; if (!t.EndsWith("}")) t = t + "}"; return t; })
                                .ToArray();
                return parts;
            }
            return new[] { json };
        }

        // --------- Stolichny JSON (compact) ---------
        public static async Task<decimal?> FetchPriceFromStolichnyRynokAsync(string itemQuery, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(itemQuery)) return null;
            List<StolichEntry> all;
            lock (_stolLock)
            {
                if (_stolichCache != null && (DateTime.UtcNow - _stolichCachedAt) < StolichCacheTtl)
                {
                    all = _stolichCache;
                }
                else all = new List<StolichEntry>();
            }

            if (all.Count == 0)
            {
                var combined = new List<StolichEntry>();
                foreach (var u in StolichJsonUrls)
                {
                    try
                    {
                        using var r = await _http.GetAsync(u, ct).ConfigureAwait(false);
                        r.EnsureSuccessStatusCode();
                        var s = await r.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        // теперь десериализуем в StolichEntry с JsonElement-полями
                        var arr = JsonSerializer.Deserialize<List<StolichEntry>>(s, opts);
                        if (arr != null) combined.AddRange(arr);
                        else
                        {
                            try { DebugLogger.Log($"Stolich URL returned no array: {u}"); } catch { }
                        }
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        // логируем ошибку загрузки/парсинга — важно для диагностики
                        try { DebugLogger.LogError($"Failed loading/parsing Stolich URL: {u}", ex); } catch { }
                    }
                }
                lock (_stolLock) { _stolichCache = combined; _stolichCachedAt = DateTime.UtcNow; all = combined; }
            }

            if (all.Count == 0) return null;

            var q = (itemQuery ?? "").Trim().ToLowerInvariant();

            // три шага поиска: exact -> contains -> startswith
            var matches = all.Where(x => ((x.item ?? "").Trim().ToLowerInvariant() == q)).ToList();
            if (matches.Count == 0) matches = all.Where(x => ((x.item ?? "").ToLowerInvariant().Contains(q))).ToList();
            if (matches.Count == 0) matches = all.Where(x => ((x.item ?? "").ToLowerInvariant().StartsWith(q))).ToList();
            if (matches.Count == 0) return null;

            // выбрать самый свежий по date
            StolichEntry? best = null; DateTime dtMax = DateTime.MinValue;
            foreach (var m in matches)
            {
                if (DateTime.TryParse(m.date, out var dt))
                {
                    if (dt > dtMax) { dtMax = dt; best = m; }
                }
                else if (best == null) best = m;
            }

            if (best == null) return null;

            // Попытка получить price2, fallback на price1/price3
            var price = TryParsePrice(best.price2) ?? TryParsePrice(best.price1) ?? TryParsePrice(best.price3);
            return price;
        }

        // stub
        public static Task<decimal?> FetchPriceFromVelykaKyshenyaAsync(string url, CancellationToken ct) => Task.FromResult<decimal?>(null);

        public static async Task<decimal?> FetchPriceFromNovusAsync(string url, CancellationToken ct)
        {
            return await FetchPriceFromMetroAsync(url, ct).ConfigureAwait(false);
        }
        static decimal? TryParsePrice(JsonElement el)
        {
            try
            {
                switch (el.ValueKind)
                {
                    case JsonValueKind.Number:
                        if (el.TryGetDecimal(out var d)) return d;
                        return null;
                    case JsonValueKind.String:
                        var s = el.GetString();
                        if (string.IsNullOrWhiteSpace(s)) return null;
                        s = s.Replace(',', '.').Trim();
                        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d2)) return d2;
                        return null;
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        // DTO
        private class StolichEntry
        {
            public string? date { get; set; }
            public string? category { get; set; }
            public string? item { get; set; }
            public JsonElement price1 { get; set; }
            public JsonElement price2 { get; set; }
            public JsonElement price3 { get; set; }
        }
    }
}