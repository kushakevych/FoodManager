using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace FoodManager.Services.Scrapers
{
    /// <summary>
    /// Thread-safe browser pool for headless web automation.
    /// </summary>
    public interface IBrowserPool
    {
        bool IsInitialized { get; }
        Task EnsureInitializedAsync(int maxParallel = 4);
        Task<T> RunWithPageAsync<T>(Func<IPage, Task<T>> pageAction, CancellationToken ct);
        Task DisposeAsync();
    }
}
