using System;
using System.Threading;
using System.Threading.Tasks;
using FoodManager.Services.Logging;
using FoodManager.Services.Scrapers;

namespace FoodManager
{
    /// <summary>
    /// Debug facade delegating to <see cref="PriceScraperService.Default"/> with full operation logging.
    /// </summary>
    public static class PriceUpdaterDebug
    {
        public static void StartDebugLog()
        {
            FileAppLogger.Default.Clear();
            FileAppLogger.Default.Log("Debug log initialized.");
        }

        public static Task<bool> UpdateProductWithLogging(Product p, CancellationToken ct) =>
            PriceScraperService.Default.UpdatePricesForProductAsync(p, ct, enableLogging: true);
    }
}
