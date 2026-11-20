using System;
using System.Threading;
using System.Threading.Tasks;
using FoodManager.Common;
using FoodManager.Services.Scrapers;

namespace FoodManager
{
    /// <summary>
    /// Facade providing backward compatibility with existing store fetchers and updaters.
    /// Delegates to <see cref="PriceScraperService.Default"/>.
    /// </summary>
    public static class PriceUpdaters
    {
        public static readonly string StoreATB = StoreConstants.ATB;
        public static readonly string StoreSilpo = StoreConstants.Silpo;
        public static readonly string StoreStolich = StoreConstants.Stolichny;
        public static readonly string StoreMetro = StoreConstants.Metro;
        public static readonly string StoreVK = StoreConstants.VelykaKyshenya;
        public static readonly string StoreAshan = StoreConstants.Ashan;
        public static readonly string StoreNovus = StoreConstants.Novus;

        public static Task EnsurePlaywrightAsync(int maxParallel = 4) =>
            PriceScraperService.Default.EnsureInitializedAsync(maxParallel);

        public static Task DisposePlaywrightAsync() =>
            PriceScraperService.Default.DisposeAsync();

        public static Task<bool> UpdatePricesForProductAsync(Product p, CancellationToken ct) =>
            PriceScraperService.Default.UpdatePricesForProductAsync(p, ct, enableLogging: false);

        public static Task<decimal?> FetchPriceFromAtbAsync(string url, CancellationToken ct) =>
            PriceScraperService.Default.FetchPriceFromAtbAsync(url, ct);

        public static Task<decimal?> FetchPriceFromSilpoAsync(string url, CancellationToken ct) =>
            PriceScraperService.Default.FetchPriceFromSilpoAsync(url, ct);

        public static Task<decimal?> FetchPriceFromMetroAsync(string url, CancellationToken ct) =>
            PriceScraperService.Default.FetchPriceFromMetroAsync(url, ct);

        public static Task<decimal?> FetchPriceFromStolichnyRynokAsync(string itemQuery, CancellationToken ct) =>
            PriceScraperService.Default.FetchPriceFromStolichnyRynokAsync(itemQuery, ct);

        public static Task<decimal?> FetchPriceFromNovusAsync(string url, CancellationToken ct) =>
            PriceScraperService.Default.FetchPriceFromNovusAsync(url, ct);

        public static Task<decimal?> FetchPriceFromAshanAsync(string url, CancellationToken ct) =>
            PriceScraperService.Default.FetchPriceFromAshanAsync(url, ct);

        public static Task<decimal?> FetchPriceFromVelykaKyshenyaAsync(string url, CancellationToken ct) =>
            PriceScraperService.Default.FetchPriceFromVelykaKyshenyaAsync(url, ct);
    }
}
