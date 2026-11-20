using System.Threading;
using System.Threading.Tasks;

namespace FoodManager.Services.Scrapers
{
    /// <summary>
    /// Service for scraping and updating product prices across supermarkets and online markets.
    /// </summary>
    public interface IPriceScraperService
    {
        Task EnsureInitializedAsync(int maxParallel = 4);
        Task DisposeAsync();

        Task<bool> UpdatePricesForProductAsync(Product p, CancellationToken ct, bool enableLogging = true);

        Task<decimal?> FetchPriceFromAtbAsync(string url, CancellationToken ct);
        Task<decimal?> FetchPriceFromSilpoAsync(string url, CancellationToken ct);
        Task<decimal?> FetchPriceFromMetroAsync(string url, CancellationToken ct);
        Task<decimal?> FetchPriceFromStolichnyRynokAsync(string itemQuery, CancellationToken ct);
        Task<decimal?> FetchPriceFromNovusAsync(string url, CancellationToken ct);
        Task<decimal?> FetchPriceFromAshanAsync(string url, CancellationToken ct);
        Task<decimal?> FetchPriceFromVelykaKyshenyaAsync(string url, CancellationToken ct);
    }
}
