using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoodManager
{
    /// <summary>
    /// Debug wrapper for <see cref="PriceUpdaters"/> that logs every fetch step.
    /// Mirrors UpdatePricesForProductAsync but runs stores IN PARALLEL with per-store timing.
    /// </summary>
    public static class PriceUpdaterDebug
    {
        // Maps each store to its fetcher — keeps in sync with PriceUpdaters.Fetchers automatically
        // by delegating to the same public methods.
        private static readonly IReadOnlyDictionary<string, Func<string, CancellationToken, Task<decimal?>>> Fetchers =
            new Dictionary<string, Func<string, CancellationToken, Task<decimal?>>>(StringComparer.OrdinalIgnoreCase)
            {
                [PriceUpdaters.StoreATB]     = PriceUpdaters.FetchPriceFromAtbAsync,
                [PriceUpdaters.StoreSilpo]   = PriceUpdaters.FetchPriceFromSilpoAsync,
                [PriceUpdaters.StoreStolich] = PriceUpdaters.FetchPriceFromStolichnyRynokAsync,
                [PriceUpdaters.StoreMetro]   = PriceUpdaters.FetchPriceFromMetroAsync,
                [PriceUpdaters.StoreVK]      = PriceUpdaters.FetchPriceFromVelykaKyshenyaAsync,
                [PriceUpdaters.StoreAshan]   = PriceUpdaters.FetchPriceFromMetroAsync,
                [PriceUpdaters.StoreNovus]   = PriceUpdaters.FetchPriceFromNovusAsync,
            };

        public static void StartDebugLog()
        {
            DebugLogger.Clear();
            DebugLogger.Log("Debug log initialized.");
        }

        /// <summary>
        /// Updates all store prices IN PARALLEL, logging every step with elapsed timings.
        /// Returns <c>true</c> if any price changed.
        /// </summary>
        public static async Task<bool> UpdateProductWithLogging(Product p, CancellationToken ct)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            DebugLogger.Log($"START product id={p.Id} name='{p.Name}'");

            var links  = p.Links             ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var manual = p.ManualPriceSources ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Build one task per eligible store
            var storeTasks = Fetchers
                .Where(kv => !manual.Contains(kv.Key)
                          && links.TryGetValue(kv.Key, out var u)
                          && !string.IsNullOrWhiteSpace(u))
                .Select(kv => FetchStoreWithLoggingAsync(p, kv.Key, kv.Value, links[kv.Key], ct))
                .ToList();

            var changed = false;
            try
            {
                var results = await Task.WhenAll(storeTasks).ConfigureAwait(false);
                changed = results.Any(r => r);
            }
            catch (OperationCanceledException)
            {
                DebugLogger.Log($"CANCEL product id={p.Id} name='{p.Name}'");
                throw;
            }
            catch (Exception ex)
            {
                DebugLogger.LogError($"Unhandled error while updating product id={p.Id} name='{p.Name}'", ex);
            }
            finally
            {
                DebugLogger.Log($"END product id={p.Id} name='{p.Name}' changed={changed}");
            }

            return changed;
        }

        // ── Per-store logging wrapper ─────────────────────────────────────────

        private static async Task<bool> FetchStoreWithLoggingAsync(
            Product p, string storeName,
            Func<string, CancellationToken, Task<decimal?>> fetcher,
            string arg, CancellationToken ct)
        {
            // Truncate long arguments in log output
            var argPreview = arg.Length > 120 ? arg[..120] + "..." : arg;

            DebugLogger.Log($"CALL {storeName} start (arg='{argPreview}')");
            var sw = Stopwatch.StartNew();

            decimal? price = null;
            try
            {
                price = await fetcher(arg, ct).ConfigureAwait(false);
                sw.Stop();
                DebugLogger.Log($"CALL {storeName} done in {sw.ElapsedMilliseconds} ms => {(price.HasValue ? price.Value.ToString("0.##") : "NULL")}");
            }
            catch (OperationCanceledException)
            {
                sw.Stop();
                DebugLogger.Log($"CALL {storeName} cancelled after {sw.ElapsedMilliseconds} ms");
                throw;
            }
            catch (Exception ex)
            {
                sw.Stop();
                DebugLogger.LogError($"CALL {storeName} failed after {sw.ElapsedMilliseconds} ms", ex);
                return false;
            }

            if (!price.HasValue) return false;

            // Thread-safe read + write — check if value actually changed
            bool changed = false;
            lock (p)
            {
                p.Prices ??= new Dictionary<string, decimal>();
                if (!p.Prices.TryGetValue(storeName, out var old) || old != price.Value)
                {
                    p.Prices[storeName] = price.Value;
                    changed = true;
                }
            }

            if (changed)
                DebugLogger.Log($"UPDATE {p.Name} {storeName} => {price.Value}");

            return changed;
        }
    }
}
