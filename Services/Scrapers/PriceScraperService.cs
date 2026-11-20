using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FoodManager.Common;
using FoodManager.Services.Logging;
using Microsoft.Playwright;

namespace FoodManager.Services.Scrapers
{
    /// <summary>
    /// High-performance multi-store price scraper implementation.
    /// Handles concurrent fetching, browser automation, JSON-LD extraction and caching.
    /// </summary>
    public class PriceScraperService : IPriceScraperService
    {
        private static readonly Lazy<PriceScraperService> _instance = new(() => new PriceScraperService());
        public static PriceScraperService Default => _instance.Value;

        private readonly IBrowserPool _browserPool;
        private readonly IAppLogger _logger;
        private readonly HttpClient _http;

        private readonly IReadOnlyDictionary<string, Func<string, CancellationToken, Task<decimal?>>> _fetchers;

        // Stolichny market JSON cache
        private static readonly string[] StolichJsonUrls =
        {
            "https://kyivopt.com/assets/cache_google/data_cache_fish_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_fruits_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_grocery_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_meat_0.json",
            "https://kyivopt.com/assets/cache_google/data_cache_milk_0.json",
        };

        private List<StolichEntry>? _stolichCache;
        private DateTime _stolichCachedAt = DateTime.MinValue;
        private static readonly TimeSpan StolichCacheTtl = TimeSpan.FromMinutes(60);
        private readonly SemaphoreSlim _stolLock = new(1, 1);

        // Compiled regexes
        private static readonly Regex RxPrice = new(@"\d+(?:[.,]\d{2})?", RegexOptions.Compiled);
        private static readonly Regex RxPriceParts = new(@"(\d+)[^\d]+(\d{2})", RegexOptions.Compiled);
        private static readonly Regex RxScriptLdJson = new(
            @"<script[^>]*type\s*=\s*[""']application\/ld\+json[""'][^>]*>(.*?)<\/script>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RxOffersPrice = new(
            @"""offers""\s*:\s*\{[^}]*?""price""\s*:\s*[""']?([0-9]+(?:[.,][0-9]+))[""']?",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        private readonly object _priceLock = new();

        public PriceScraperService(IBrowserPool? browserPool = null, IAppLogger? logger = null, HttpClient? httpClient = null)
        {
            _browserPool = browserPool ?? BrowserPool.Shared;
            _logger = logger ?? FileAppLogger.Default;
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

            _fetchers = new Dictionary<string, Func<string, CancellationToken, Task<decimal?>>>(StringComparer.OrdinalIgnoreCase)
            {
                [StoreConstants.ATB] = FetchPriceFromAtbAsync,
                [StoreConstants.Silpo] = FetchPriceFromSilpoAsync,
                [StoreConstants.Stolichny] = FetchPriceFromStolichnyRynokAsync,
                [StoreConstants.Metro] = FetchPriceFromMetroAsync,
                [StoreConstants.VelykaKyshenya] = FetchPriceFromVelykaKyshenyaAsync,
                [StoreConstants.Ashan] = FetchPriceFromAshanAsync,
                [StoreConstants.Novus] = FetchPriceFromNovusAsync,
            };
        }

        public async Task EnsureInitializedAsync(int maxParallel = 4)
        {
            await _browserPool.EnsureInitializedAsync(maxParallel).ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await _browserPool.DisposeAsync().ConfigureAwait(false);
        }

        public async Task<bool> UpdatePricesForProductAsync(Product p, CancellationToken ct, bool enableLogging = true)
        {
            ArgumentNullException.ThrowIfNull(p);

            if (enableLogging)
            {
                _logger.Log($"START product id={p.Id} name='{p.Name}'");
            }

            var links = p.Links ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var manual = p.ManualPriceSources ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var storeTasks = _fetchers
                .Where(kv => !manual.Contains(kv.Key)
                          && links.TryGetValue(kv.Key, out var u)
                          && !string.IsNullOrWhiteSpace(u))
                .Select(kv => FetchStorePriceAsync(p, kv.Key, kv.Value, links[kv.Key], ct, enableLogging))
                .ToList();

            var changed = false;
            try
            {
                var results = await Task.WhenAll(storeTasks).ConfigureAwait(false);
                changed = results.Any(r => r);
            }
            catch (OperationCanceledException)
            {
                if (enableLogging) _logger.Log($"CANCEL product id={p.Id} name='{p.Name}'");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unhandled error while updating product id={p.Id} name='{p.Name}'", ex);
            }
            finally
            {
                if (enableLogging) _logger.Log($"END product id={p.Id} name='{p.Name}' changed={changed}");
            }

            return changed;
        }

        private async Task<bool> FetchStorePriceAsync(
            Product p, string storeName,
            Func<string, CancellationToken, Task<decimal?>> fetcher,
            string arg, CancellationToken ct, bool enableLogging)
        {
            var sw = Stopwatch.StartNew();
            if (enableLogging)
            {
                var argPreview = arg.Length > 120 ? arg[..120] + "..." : arg;
                _logger.Log($"CALL {storeName} start (arg='{argPreview}')");
            }

            try
            {
                var price = await fetcher(arg, ct).ConfigureAwait(false);
                sw.Stop();

                if (enableLogging)
                {
                    _logger.Log($"CALL {storeName} done in {sw.ElapsedMilliseconds} ms => {(price.HasValue ? price.Value.ToString("0.##", CultureInfo.InvariantCulture) : "NULL")}");
                }

                if (price.HasValue)
                {
                    return UpsertPrice(p, storeName, price.Value);
                }
                return false;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError($"Fetch failed: store={storeName} arg='{arg}'", ex);
                return false;
            }
        }

        private bool UpsertPrice(Product p, string store, decimal price)
        {
            lock (_priceLock)
            {
                p.Prices ??= new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                if (p.Prices.TryGetValue(store, out var oldPrice) && oldPrice == price)
                {
                    return false;
                }
                p.Prices[store] = price;
                return true;
            }
        }

        #region Store Implementations

        public Task<decimal?> FetchPriceFromAtbAsync(string url, CancellationToken ct)
            => FetchPriceViaPlaywrightAsync(url, ".product-price__top", ".product-price__coin", ct, StoreConstants.ATB);

        public Task<decimal?> FetchPriceFromSilpoAsync(string url, CancellationToken ct)
            => FetchPriceViaPlaywrightAsync(url, ".main-price.ng-star-inserted", null, ct, StoreConstants.Silpo);

        public Task<decimal?> FetchPriceFromVelykaKyshenyaAsync(string url, CancellationToken ct)
            => Task.FromResult<decimal?>(null);

        public Task<decimal?> FetchPriceFromNovusAsync(string url, CancellationToken ct)
            => FetchPriceFromMetroAsync(url, ct);

        public Task<decimal?> FetchPriceFromAshanAsync(string url, CancellationToken ct)
            => FetchPriceFromMetroAsync(url, ct);

        public async Task<decimal?> FetchPriceFromMetroAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseContentRead, ct).ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();
                var html = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(html)) return null;

                // 1. JSON-LD parsing
                foreach (Match m in RxScriptLdJson.Matches(html))
                {
                    var json = m.Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(json)) continue;

                    var price = TryExtractFromJsonLd(json);
                    if (price.HasValue) return price;
                }

                // 2. Regex offers fallback
                var ofm = RxOffersPrice.Match(html);
                if (ofm.Success)
                {
                    var s = ofm.Groups[1].Value.Replace(',', '.');
                    if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                    {
                        return v;
                    }
                }

                return null;
            }
            catch (OperationCanceledException) { throw; }
            catch { return null; }
        }

        public async Task<decimal?> FetchPriceFromStolichnyRynokAsync(string itemQuery, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(itemQuery)) return null;

            var all = await GetStolichCacheAsync(ct).ConfigureAwait(false);
            if (all.Count == 0) return null;

            var q = itemQuery.Trim().ToLowerInvariant();

            // Match exact -> contains -> starts with
            var matches = all.Where(x => string.Equals((x.item ?? string.Empty).Trim(), q, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) matches = all.Where(x => (x.item ?? string.Empty).Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) matches = all.Where(x => (x.item ?? string.Empty).StartsWith(q, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) return null;

            var best = matches
                .Select(m => (entry: m, date: DateTime.TryParse(m.date, out var d) ? d : DateTime.MinValue))
                .OrderByDescending(t => t.date)
                .First()
                .entry;

            return TryParsePrice(best.price2) ?? TryParsePrice(best.price1) ?? TryParsePrice(best.price3);
        }

        private async Task<List<StolichEntry>> GetStolichCacheAsync(CancellationToken ct)
        {
            await _stolLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_stolichCache != null && (DateTime.UtcNow - _stolichCachedAt) < StolichCacheTtl)
                {
                    return _stolichCache;
                }

                var downloadTasks = StolichJsonUrls.Select(u => DownloadStolichUrlAsync(u, ct));
                var batches = await Task.WhenAll(downloadTasks).ConfigureAwait(false);
                var combined = batches.SelectMany(b => b).ToList();

                _stolichCache = combined;
                _stolichCachedAt = DateTime.UtcNow;
                return combined;
            }
            finally
            {
                _stolLock.Release();
            }
        }

        private async Task<List<StolichEntry>> DownloadStolichUrlAsync(string url, CancellationToken ct)
        {
            try
            {
                using var r = await _http.GetAsync(url, ct).ConfigureAwait(false);
                r.EnsureSuccessStatusCode();
                var s = await r.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<StolichEntry>>(s, opts) ?? new List<StolichEntry>();
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError($"Failed loading Stolich URL: {url}", ex);
                return new List<StolichEntry>();
            }
        }

        private async Task<decimal?> FetchPriceViaPlaywrightAsync(
            string url,
            string primarySelector,
            string? coinSelector,
            CancellationToken ct,
            string storeName)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));
            var token = cts.Token;

            try
            {
                return await _browserPool.RunWithPageAsync(async page =>
                {
                    token.ThrowIfCancellationRequested();
                    var sw = Stopwatch.StartNew();

                    await page.GotoAsync(url, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.DOMContentLoaded,
                        Timeout = 8_000
                    }).ConfigureAwait(false);

                    var locator = page.Locator(primarySelector).First;
                    try
                    {
                        await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = 3_000 }).ConfigureAwait(false);
                    }
                    catch (TimeoutException)
                    {
                        var alt = page.Locator(".product-price, .price, [data-testid=\"product-price\"]");
                        try
                        {
                            await alt.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 2_000 }).ConfigureAwait(false);
                            locator = alt.First;
                        }
                        catch { }
                    }

                    string? raw = null;
                    try { raw = await TryGetAttrOrText(locator, "aria-label").ConfigureAwait(false); } catch { }

                    if (string.IsNullOrWhiteSpace(raw) && coinSelector != null)
                    {
                        var intPart = await TryInnerTextSafe(locator.Locator("span").First).ConfigureAwait(false);
                        var coinLoc = locator.Locator(coinSelector);
                        var coinPart = await coinLoc.CountAsync().ConfigureAwait(false) > 0
                            ? await TryInnerTextSafe(coinLoc.First).ConfigureAwait(false)
                            : null;

                        if (!string.IsNullOrWhiteSpace(intPart) && !string.IsNullOrWhiteSpace(coinPart))
                        {
                            var intOnly = Regex.Match(intPart, @"\d+").Value;
                            raw = $"{intOnly}.{coinPart}";
                        }
                    }

                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        raw = await TryInnerTextSafe(locator).ConfigureAwait(false);
                    }

                    sw.Stop();
                    _logger.Log($"{storeName} fetch: {url} elapsed={sw.Elapsed.TotalSeconds:F2}s raw='{raw}'");
                    return ParsePrice(raw);
                }, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError($"{storeName} fetch failed for {url}", ex);
                return null;
            }
        }

        #endregion

        #region Helpers

        public static decimal? ParsePrice(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            raw = raw.Replace("\u00A0", " ");

            // 1. "31 50", "31-50", "31.50", "31,50" style with explicit decimal/coin parts
            var m2 = RxPriceParts.Match(raw);
            if (m2.Success)
            {
                var norm = $"{m2.Groups[1].Value}.{m2.Groups[2].Value}";
                if (decimal.TryParse(norm, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                {
                    return v;
                }
            }

            // 2. Single integer or decimal
            var m = RxPrice.Match(raw);
            if (m.Success)
            {
                var norm = m.Value.Replace(',', '.');
                if (decimal.TryParse(norm, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                {
                    return v;
                }
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
                    _ => null
                };
            }
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
            {
                return TryPriceFromElement(nested, out price);
            }

            if (e.TryGetProperty("price", out var pe))
            {
                price = pe.ValueKind switch
                {
                    JsonValueKind.String => ParsePrice(pe.GetString()),
                    JsonValueKind.Number => pe.TryGetDecimal(out var v) ? v : null,
                    _ => null
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
                    if (!t.StartsWith('{')) t = "{" + t;
                    if (!t.EndsWith('}')) t += "}";
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

        private sealed class StolichEntry
        {
            public string? date { get; set; }
            public string? category { get; set; }
            public string? item { get; set; }
            public JsonElement price1 { get; set; }
            public JsonElement price2 { get; set; }
            public JsonElement price3 { get; set; }
        }

        #endregion
    }
}
