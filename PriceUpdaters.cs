using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
        // ── Store name constants ──────────────────────────────────────────────
        public static readonly string StoreATB     = "АТБ";
        public static readonly string StoreSilpo   = "Сільпо";
        public static readonly string StoreStolich = "Столичний ринок";
        public static readonly string StoreMetro   = "Метро";
        public static readonly string StoreVK      = "Велика Кишеня";
        public static readonly string StoreAshan   = "Ашан";
        public static readonly string StoreNovus   = "NOVUS";

        // ── Fetcher registry ─────────────────────────────────────────────────
        // Maps each store name to its fetch delegate.
        // Add or remove stores here — UpdatePricesForProductAsync picks them up automatically.
        private static readonly IReadOnlyDictionary<string, Func<string, CancellationToken, Task<decimal?>>> Fetchers =
            new Dictionary<string, Func<string, CancellationToken, Task<decimal?>>>(StringComparer.OrdinalIgnoreCase)
            {
                [StoreATB]     = FetchPriceFromAtbAsync,
                [StoreSilpo]   = FetchPriceFromSilpoAsync,
                [StoreStolich] = FetchPriceFromStolichnyRynokAsync,
                [StoreMetro]   = FetchPriceFromMetroAsync,
                [StoreVK]      = FetchPriceFromVelykaKyshenyaAsync,
                [StoreAshan]   = FetchPriceFromMetroAsync,   // Ashan reuses Metro parser
                [StoreNovus]   = FetchPriceFromMetroAsync,   // Novus  reuses Metro parser
            };

        // ── Shared HTTP client ────────────────────────────────────────────────
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        // ── Playwright pool ───────────────────────────────────────────────────
        private static IPlaywright?      _pw;
        private static IBrowser?         _browser;
        private static IBrowserContext?  _context;
        private static SemaphoreSlim?    _pageLimiter;
        // AsyncLazy guarantees one-shot initialisation even under concurrent callers.
        private static Lazy<Task>        _pwInit = new Lazy<Task>(() => Task.CompletedTask);
        private static readonly SemaphoreSlim _pwSetupLock = new SemaphoreSlim(1, 1);

        private const int NAV_TIMEOUT_MS = 20_000;

        // ── Stolichny cache ───────────────────────────────────────────────────
        private static readonly string[] StolichJsonUrls =
        {
            "https://kyivopt.com/assets/cache_google/data_cache_fish_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_fruits_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_grocery_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_meat_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_milk_0.json",
        };

        private static List<StolichEntry>? _stolichCache;
        private static DateTime            _stolichCachedAt = DateTime.MinValue;
        private static readonly TimeSpan   StolichCacheTtl  = TimeSpan.FromMinutes(60);
        // SemaphoreSlim(1,1) replaces lock{} so the slow HTTP fetch doesn't block threads.
        private static readonly SemaphoreSlim _stolLock = new SemaphoreSlim(1, 1);

        // ── Compiled regexes (avoid recompiling on every call) ────────────────
        private static readonly Regex RxPrice         = new Regex(@"\d+(?:[.,]\d{2})?",    RegexOptions.Compiled);
        private static readonly Regex RxPriceParts    = new Regex(@"(\d+)[^\d]+(\d{2})",   RegexOptions.Compiled);
        private static readonly Regex RxDecimal       = new Regex(@"(\d+(?:[.,]\d+)?)",    RegexOptions.Compiled);
        private static readonly Regex RxScriptLdJson  = new Regex(
            @"<script[^>]*type\s*=\s*[""']application\/ld\+json[""'][^>]*>(.*?)<\/script>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxOffersPrice   = new Regex(
            @"""offers""\s*:\s*\{[^}]*?""price""\s*:\s*[""']?([0-9]+(?:[.,][0-9]+))[""']?",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        // ═════════════════════════════════════════════════════════════════════
        // Lifecycle
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Call once before running Playwright-based fetchers.
        /// Thread-safe: concurrent callers wait for the single initialisation to complete.
        /// </summary>
        public static async Task EnsurePlaywrightAsync(int maxParallel = 4)
        {
            if (_context != null) return; // fast path — already ready

            await _pwSetupLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_context != null) return; // double-checked inside lock

                _pw = await Playwright.CreateAsync().ConfigureAwait(false);
                _browser = await _pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[]
                    {
                        "--disable-dev-shm-usage",
                        "--disable-extensions",
                        "--disable-gpu",
                        "--no-sandbox",
                        "--disable-background-networking",
                    },
                }).ConfigureAwait(false);

                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    Locale    = "uk-UA",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
                }).ConfigureAwait(false);

                _pageLimiter = new SemaphoreSlim(Math.Max(1, maxParallel), Math.Max(1, maxParallel));
            }
            finally
            {
                _pwSetupLock.Release();
            }
        }

        public static async Task DisposePlaywrightAsync()
        {
            try { if (_context  != null) await _context.CloseAsync().ConfigureAwait(false); }  catch { }
            try { if (_browser  != null) await _browser.CloseAsync().ConfigureAwait(false); }  catch { }
            try { _pw?.Dispose(); }                                                              catch { }

            _context = null; _browser = null; _pw = null;
            _pageLimiter?.Dispose(); _pageLimiter = null;
        }

        // ═════════════════════════════════════════════════════════════════════
        // High-level updater — ALL stores run IN PARALLEL
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Updates all store prices for <paramref name="p"/> concurrently.
        /// Returns <c>true</c> if any price changed.
        /// </summary>
        public static async Task<bool> UpdatePricesForProductAsync(Product p, CancellationToken ct)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            var links  = p.Links             ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var manual = p.ManualPriceSources ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Build one task per store that has a link and is not manually managed.
            var tasks = Fetchers
                .Where(kv => !manual.Contains(kv.Key)
                          && links.TryGetValue(kv.Key, out var u)
                          && !string.IsNullOrWhiteSpace(u))
                .Select(kv =>
                {
                    var store   = kv.Key;
                    var fetcher = kv.Value;
                    var url     = links[store];
                    return FetchAndUpsertAsync(p, store, fetcher, url, ct);
                })
                .ToList();

            // Run all store fetches concurrently, collect changed flags.
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.Any(changed => changed);
        }

        /// <summary>
        /// Fetches a single store price and writes it to <paramref name="p"/> if it changed.
        /// Exceptions are caught and logged — never propagate (except cancellation).
        /// </summary>
        private static async Task<bool> FetchAndUpsertAsync(
            Product p, string store,
            Func<string, CancellationToken, Task<decimal?>> fetcher,
            string arg, CancellationToken ct)
        {
            try
            {
                var price = await fetcher(arg, ct).ConfigureAwait(false);
                if (price.HasValue)
                    return UpsertPrice(p, store, price.Value);
                return false;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                try { DebugLogger.LogError($"FetchAndUpsert failed: store={store} arg='{arg}'", ex); } catch { }
                return false;
            }
        }

        // Thread-safe price upsert (p.Prices is a shared Dictionary; lock on p or use ConcurrentDictionary in caller).
        private static readonly object _priceLock = new object();
        private static bool UpsertPrice(Product p, string store, decimal price)
        {
            lock (_priceLock)
            {
                p.Prices ??= new Dictionary<string, decimal>();
                if (p.Prices.TryGetValue(store, out var old) && old == price) return false;
                p.Prices[store] = price;
                return true;
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // Playwright page helper
        // ═════════════════════════════════════════════════════════════════════

        private static async Task<T> RunWithPageAsync<T>(Func<IPage, Task<T>> action, CancellationToken ct)
        {
            if (_context == null || _pageLimiter == null)
                throw new InvalidOperationException("Playwright not initialized. Call EnsurePlaywrightAsync first.");

            await _pageLimiter.WaitAsync(ct).ConfigureAwait(false);
            IPage? page = null;
            try
            {
                page = await _context.NewPageAsync().ConfigureAwait(false);

                // Block heavy resources to reduce bandwidth and speed up page loads.
                await page.RouteAsync("**/*", async route =>
                {
                    try
                    {
                        var t = route.Request.ResourceType;
                        if (t is "image" or "font" or "media")
                            await route.AbortAsync();
                        else
                            await route.ContinueAsync();
                    }
                    catch
                    {
                        try { await route.ContinueAsync(); } catch { }
                    }
                }).ConfigureAwait(false);

                return await action(page).ConfigureAwait(false);
            }
            finally
            {
                if (page != null)
                    try { await page.CloseAsync().ConfigureAwait(false); } catch { }
                _pageLimiter.Release();
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // Generic Playwright fetcher (shared by ATB & Silpo)
        // ═════════════════════════════════════════════════════════════════════

        private static async Task<decimal?> FetchPriceViaPlaywrightAsync(
            string url,
            string primarySelector,
            string? coinSelector,
            CancellationToken ct,
            string storeName)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(8));
            var token = cts.Token;

            try
            {
                return await RunWithPageAsync(async page =>
                {
                    token.ThrowIfCancellationRequested();
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        await page.GotoAsync(url, new PageGotoOptions
                        {
                            WaitUntil = WaitUntilState.DOMContentLoaded,
                            Timeout   = Math.Min(NAV_TIMEOUT_MS, 8_000),
                        }).ConfigureAwait(false);

                        var locator = page.Locator(primarySelector).First;
                        try
                        {
                            await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = 3_000 }).ConfigureAwait(false);
                        }
                        catch (TimeoutException)
                        {
                            // Fallback to generic price selectors
                            var alt = page.Locator(".product-price, .price, [data-testid=\"product-price\"]");
                            try
                            {
                                await alt.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 2_000 }).ConfigureAwait(false);
                                locator = alt.First;
                            }
                            catch { /* continue with original locator */ }
                        }

                        string? raw = null;
                        // 1. Try aria-label (most reliable, single-string price)
                        try { raw = await TryGetAttrOrText(locator, "aria-label").ConfigureAwait(false); } catch { }

                        // 2. For ATB-style int+coin split
                        if (string.IsNullOrWhiteSpace(raw) && coinSelector != null)
                        {
                            var intPart  = await TryInnerTextSafe(locator.Locator("span").First).ConfigureAwait(false);
                            var coinLoc  = locator.Locator(coinSelector);
                            var coinPart = await coinLoc.CountAsync().ConfigureAwait(false) > 0
                                ? await TryInnerTextSafe(coinLoc.First).ConfigureAwait(false)
                                : null;

                            if (!string.IsNullOrWhiteSpace(intPart) && !string.IsNullOrWhiteSpace(coinPart))
                            {
                                var intOnly = Regex.Match(intPart, @"\d+").Value;
                                raw = $"{intOnly}.{coinPart}";
                            }
                        }

                        // 3. Fallback: full inner text
                        if (string.IsNullOrWhiteSpace(raw))
                            raw = await TryInnerTextSafe(locator).ConfigureAwait(false);

                        sw.Stop();
                        try { DebugLogger.Log($"{storeName} fetch: {url} elapsed={sw.Elapsed.TotalSeconds:F2}s raw='{raw}'"); } catch { }

                        return ParsePrice(raw);
                    }
                    catch (TimeoutException tex)
                    {
                        try { DebugLogger.LogError($"{storeName} timeout for {url}", tex); } catch { }
                        return (decimal?)null;
                    }
                }, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                try { DebugLogger.LogError($"{storeName} fetch failed for {url}", ex); } catch { }
                return null;
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // Store-specific fetchers
        // ═════════════════════════════════════════════════════════════════════

        public static Task<decimal?> FetchPriceFromAtbAsync(string url, CancellationToken ct)
            => FetchPriceViaPlaywrightAsync(url, ".product-price__top", ".product-price__coin", ct, "ATB");

        public static Task<decimal?> FetchPriceFromSilpoAsync(string url, CancellationToken ct)
            => FetchPriceViaPlaywrightAsync(url, ".main-price.ng-star-inserted", null, ct, "Silpo");

        // Велика Кишеня — stub (site retired / requires investigation)
        public static Task<decimal?> FetchPriceFromVelykaKyshenyaAsync(string url, CancellationToken ct)
            => Task.FromResult<decimal?>(null);

        // Novus reuses Metro parser
        public static Task<decimal?> FetchPriceFromNovusAsync(string url, CancellationToken ct)
            => FetchPriceFromMetroAsync(url, ct);

        // ── Metro: HttpClient + JSON-LD ──────────────────────────────────────
        public static async Task<decimal?> FetchPriceFromMetroAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            try
            {
                using var req  = new HttpRequestMessage(HttpMethod.Get, url);
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseContentRead, ct).ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();
                var html = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(html)) return null;

                // 1. Parse JSON-LD blocks
                foreach (Match m in RxScriptLdJson.Matches(html))
                {
                    var json = m.Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(json)) continue;

                    var price = TryExtractFromJsonLd(json);
                    if (price.HasValue) return price;
                }

                // 2. Regex fallback on raw HTML
                var ofm = RxOffersPrice.Match(html);
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

        private static decimal? TryExtractFromJsonLd(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return ExtractOffersPrice(doc.RootElement);
            }
            catch
            {
                // Concatenated JSON objects (e.g. "}{") — split and retry
                foreach (var part in SplitConcatenatedJson(json))
                {
                    try
                    {
                        using var doc2 = JsonDocument.Parse(part);
                        var price = ExtractOffersPrice(doc2.RootElement);
                        if (price.HasValue) return price;
                    }
                    catch { }
                }
                return null;
            }
        }

        // ── Stolichny JSON cache ─────────────────────────────────────────────
        public static async Task<decimal?> FetchPriceFromStolichnyRynokAsync(string itemQuery, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(itemQuery)) return null;

            var all = await GetStolichCacheAsync(ct).ConfigureAwait(false);
            if (all.Count == 0) return null;

            var q = itemQuery.Trim().ToLowerInvariant();

            // Three-pass search: exact → contains → starts-with
            var matches = all.Where(x => string.Equals((x.item ?? "").Trim(), q, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) matches = all.Where(x => (x.item ?? "").Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) matches = all.Where(x => (x.item ?? "").StartsWith(q, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) return null;

            // Pick the most recent entry by date
            var best = matches
                .Select(m => (entry: m, date: DateTime.TryParse(m.date, out var d) ? d : DateTime.MinValue))
                .OrderByDescending(t => t.date)
                .First()
                .entry;

            return TryParsePrice(best.price2) ?? TryParsePrice(best.price1) ?? TryParsePrice(best.price3);
        }

        /// <summary>
        /// Returns the Stolichny cache, refreshing it if expired.
        /// Async-safe: only one download runs at a time.
        /// </summary>
        private static async Task<List<StolichEntry>> GetStolichCacheAsync(CancellationToken ct)
        {
            await _stolLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_stolichCache != null && (DateTime.UtcNow - _stolichCachedAt) < StolichCacheTtl)
                    return _stolichCache;

                // Download all JSON files in parallel
                var downloadTasks = StolichJsonUrls.Select(u => DownloadStolichUrlAsync(u, ct));
                var batches       = await Task.WhenAll(downloadTasks).ConfigureAwait(false);
                var combined      = batches.SelectMany(b => b).ToList();

                _stolichCache    = combined;
                _stolichCachedAt = DateTime.UtcNow;
                return combined;
            }
            finally
            {
                _stolLock.Release();
            }
        }

        private static async Task<List<StolichEntry>> DownloadStolichUrlAsync(string url, CancellationToken ct)
        {
            try
            {
                using var r = await _http.GetAsync(url, ct).ConfigureAwait(false);
                r.EnsureSuccessStatusCode();
                var s    = await r.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<StolichEntry>>(s, opts) ?? new List<StolichEntry>();
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                try { DebugLogger.LogError($"Failed loading Stolich URL: {url}", ex); } catch { }
                return new List<StolichEntry>();
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // Helpers
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Normalises a raw price string and parses it.
        /// Handles "31.50", "31,50", "31 50", "31\u00A050" etc.
        /// </summary>
        private static decimal? ParsePrice(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            raw = raw.Replace("\u00A0", " ");

            var m = RxPrice.Match(raw);
            if (m.Success)
            {
                var norm = m.Value.Replace(',', '.');
                if (decimal.TryParse(norm, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                    return v;
            }

            // "31 50" or "31-50" style
            var m2 = RxPriceParts.Match(raw);
            if (m2.Success)
            {
                var norm = $"{m2.Groups[1].Value}.{m2.Groups[2].Value}";
                if (decimal.TryParse(norm, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                    return v;
            }

            return null;
        }

        private static decimal? TryParsePrice(JsonElement el)
        {
            try
            {
                return el.ValueKind switch
                {
                    JsonValueKind.Number => el.TryGetDecimal(out var d) ? d : null,
                    JsonValueKind.String => ParsePrice(el.GetString()),
                    _                   => null,
                };
            }
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
                            if (TryPriceFromElement(it, out var p2)) return p2;
                    }
                }
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

            if (e.TryGetProperty("offers", out var nested))
                return TryPriceFromElement(nested, out price);

            if (e.TryGetProperty("price", out var pe))
            {
                price = pe.ValueKind switch
                {
                    JsonValueKind.String => ParsePrice(pe.GetString()),
                    JsonValueKind.Number => pe.TryGetDecimal(out var v) ? v : null,
                    _                   => null,
                };
                return price.HasValue;
            }
            return false;
        }

        private static IEnumerable<string> SplitConcatenatedJson(string json)
        {
            if (!json.Contains("}{")) return new[] { json };
            return json
                .Split(new[] { "}\n{", "}{", "}\r\n{" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p =>
                {
                    var t = p.Trim();
                    if (!t.StartsWith("{")) t = "{" + t;
                    if (!t.EndsWith("}"))   t = t + "}";
                    return t;
                });
        }

        private static async Task<string?> TryGetAttrOrText(ILocator loc, string attr)
        {
            try
            {
                var a = await loc.GetAttributeAsync(attr).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(a)) return a;
            }
            catch { }
            return await TryInnerTextSafe(loc).ConfigureAwait(false);
        }

        private static async Task<string?> TryInnerTextSafe(ILocator loc)
        {
            try { return await loc.InnerTextAsync().ConfigureAwait(false); }
            catch { return null; }
        }

        // ── DTO ──────────────────────────────────────────────────────────────
        private sealed class StolichEntry
        {
            public string?      date     { get; set; }
            public string?      category { get; set; }
            public string?      item     { get; set; }
            public JsonElement  price1   { get; set; }
            public JsonElement  price2   { get; set; }
            public JsonElement  price3   { get; set; }
        }
    }
}
