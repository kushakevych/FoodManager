using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FoodManager
{
    public static class PriceUpdaterDebug
    {
        public static void StartDebugLog()
        {
            DebugLogger.Clear();
            DebugLogger.Log("Debug log initialized.");
        }

        public static async Task<bool> UpdateProductWithLogging(Product p, CancellationToken ct)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            DebugLogger.Log($"START product id={p.Id} name='{p.Name}'");

            var changed = false;
            var links = p.Links ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var manual = p.ManualPriceSources ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            async Task<decimal?> CallAndLogAsync(string storeName, Func<string, CancellationToken, Task<decimal?>> fetcher, string? arg)
            {
                if (manual.Contains(storeName))
                {
                    DebugLogger.Log($"SKIP {storeName}: manual source set by user.");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(arg))
                {
                    DebugLogger.Log($"SKIP {storeName}: no link/value provided.");
                    return null;
                }

                var sw = Stopwatch.StartNew();
                try
                {
                    DebugLogger.Log($"CALL {storeName} start (arg='{(arg?.Length > 120 ? arg.Substring(0, 120) + "..." : arg)}')");
                    var price = await fetcher(arg, ct).ConfigureAwait(false);
                    sw.Stop();
                    DebugLogger.Log($"CALL {storeName} done in {sw.ElapsedMilliseconds} ms => {(price.HasValue ? price.Value.ToString("0.##") : "NULL")}");
                    return price;
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
                    return null;
                }
            }

            try
            {
                if (links.TryGetValue(PriceUpdaters.StoreATB, out var atbUrl))
                {
                    var price = await CallAndLogAsync(PriceUpdaters.StoreATB, PriceUpdaters.FetchPriceFromAtbAsync, atbUrl);
                    if (price.HasValue)
                    {
                        if (!p.Prices.ContainsKey(PriceUpdaters.StoreATB) || p.Prices[PriceUpdaters.StoreATB] != price.Value)
                        {
                            p.Prices[PriceUpdaters.StoreATB] = price.Value;
                            changed = true;
                            DebugLogger.Log($"UPDATE {p.Name} {PriceUpdaters.StoreATB} => {price.Value}");
                        }
                    }
                }

                if (links.TryGetValue(PriceUpdaters.StoreSilpo, out var silpoUrl))
                {
                    var price = await CallAndLogAsync(PriceUpdaters.StoreSilpo, PriceUpdaters.FetchPriceFromSilpoAsync, silpoUrl);
                    if (price.HasValue)
                    {
                        if (!p.Prices.ContainsKey(PriceUpdaters.StoreSilpo) || p.Prices[PriceUpdaters.StoreSilpo] != price.Value)
                        {
                            p.Prices[PriceUpdaters.StoreSilpo] = price.Value;
                            changed = true;
                            DebugLogger.Log($"UPDATE {p.Name} {PriceUpdaters.StoreSilpo} => {price.Value}");
                        }
                    }
                }

                if (links.TryGetValue(PriceUpdaters.StoreStolich, out var stolQuery))
                {
                    var price = await CallAndLogAsync(PriceUpdaters.StoreStolich, PriceUpdaters.FetchPriceFromStolichnyRynokAsync, stolQuery);
                    if (price.HasValue)
                    {
                        if (!p.Prices.ContainsKey(PriceUpdaters.StoreStolich) || p.Prices[PriceUpdaters.StoreStolich] != price.Value)
                        {
                            p.Prices[PriceUpdaters.StoreStolich] = price.Value;
                            changed = true;
                            DebugLogger.Log($"UPDATE {p.Name} {PriceUpdaters.StoreStolich} => {price.Value}");
                        }
                    }
                }

                if (links.TryGetValue(PriceUpdaters.StoreMetro, out var metroUrl))
                {
                    var price = await CallAndLogAsync(PriceUpdaters.StoreMetro, PriceUpdaters.FetchPriceFromMetroAsync, metroUrl);
                    if (price.HasValue)
                    {
                        if (!p.Prices.ContainsKey(PriceUpdaters.StoreMetro) || p.Prices[PriceUpdaters.StoreMetro] != price.Value)
                        {
                            p.Prices[PriceUpdaters.StoreMetro] = price.Value;
                            changed = true;
                            DebugLogger.Log($"UPDATE {p.Name} {PriceUpdaters.StoreMetro} => {price.Value}");
                        }
                    }
                }

                if (links.TryGetValue(PriceUpdaters.StoreVK, out var vkUrl))
                {
                    var price = await CallAndLogAsync(PriceUpdaters.StoreVK, PriceUpdaters.FetchPriceFromVelykaKyshenyaAsync, vkUrl);
                    if (price.HasValue)
                    {
                        if (!p.Prices.ContainsKey(PriceUpdaters.StoreVK) || p.Prices[PriceUpdaters.StoreVK] != price.Value)
                        {
                            p.Prices[PriceUpdaters.StoreVK] = price.Value;
                            changed = true;
                            DebugLogger.Log($"UPDATE {p.Name} {PriceUpdaters.StoreVK} => {price.Value}");
                        }
                    }
                }

                // Ashan call (reuse Metro parsing)
                if (links.TryGetValue(PriceUpdaters.StoreAshan, out var ashanUrl))
                {
                    var price = await CallAndLogAsync(PriceUpdaters.StoreAshan, PriceUpdaters.FetchPriceFromMetroAsync, ashanUrl);
                    if (price.HasValue)
                    {
                        if (!p.Prices.ContainsKey(PriceUpdaters.StoreAshan) || p.Prices[PriceUpdaters.StoreAshan] != price.Value)
                        {
                            p.Prices[PriceUpdaters.StoreAshan] = price.Value;
                            changed = true;
                            DebugLogger.Log($"UPDATE {p.Name} {PriceUpdaters.StoreAshan} => {price.Value}");
                        }
                    }
                }

                // Novus
                if (links.TryGetValue(PriceUpdaters.StoreNovus, out var novUrl))
                {
                    var price = await CallAndLogAsync(PriceUpdaters.StoreNovus, PriceUpdaters.FetchPriceFromNovusAsync, novUrl);
                    if (price.HasValue)
                    {
                        if (!p.Prices.ContainsKey(PriceUpdaters.StoreNovus) || p.Prices[PriceUpdaters.StoreNovus] != price.Value)
                        {
                            p.Prices[PriceUpdaters.StoreNovus] = price.Value;
                            changed = true;
                            DebugLogger.Log($"UPDATE {p.Name} {PriceUpdaters.StoreNovus} => {price.Value}");
                        }
                    }
                }
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

    }
}