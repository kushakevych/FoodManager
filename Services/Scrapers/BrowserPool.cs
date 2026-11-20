using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace FoodManager.Services.Scrapers
{
    /// <summary>
    /// Thread-safe browser pool managing a single shared Playwright Chromium instance
    /// with controlled concurrent page access and resource blocking.
    /// </summary>
    public class BrowserPool : IBrowserPool, IAsyncDisposable
    {
        private static readonly Lazy<BrowserPool> _instance = new(() => new BrowserPool());
        public static BrowserPool Shared => _instance.Value;

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private SemaphoreSlim? _pageLimiter;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public bool IsInitialized => _context != null;

        public async Task EnsureInitializedAsync(int maxParallel = 4)
        {
            if (_context != null) return;

            await _initLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_context != null) return;

                _playwright = await Playwright.CreateAsync().ConfigureAwait(false);
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[]
                    {
                        "--disable-dev-shm-usage",
                        "--disable-extensions",
                        "--disable-gpu",
                        "--no-sandbox",
                        "--disable-background-networking",
                        "--disable-default-apps"
                    }
                }).ConfigureAwait(false);

                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    Locale = "uk-UA",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
                }).ConfigureAwait(false);

                _pageLimiter = new SemaphoreSlim(Math.Max(1, maxParallel), Math.Max(1, maxParallel));
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<T> RunWithPageAsync<T>(Func<IPage, Task<T>> pageAction, CancellationToken ct)
        {
            if (_context == null || _pageLimiter == null)
            {
                throw new InvalidOperationException("BrowserPool is not initialized. Call EnsureInitializedAsync first.");
            }

            await _pageLimiter.WaitAsync(ct).ConfigureAwait(false);
            IPage? page = null;
            try
            {
                page = await _context.NewPageAsync().ConfigureAwait(false);

                // Block unnecessary media resources to speed up page loads and save bandwidth
                await page.RouteAsync("**/*", async route =>
                {
                    try
                    {
                        var resourceType = route.Request.ResourceType;
                        if (resourceType is "image" or "font" or "media")
                        {
                            await route.AbortAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            await route.ContinueAsync().ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        try { await route.ContinueAsync().ConfigureAwait(false); } catch { }
                    }
                }).ConfigureAwait(false);

                return await pageAction(page).ConfigureAwait(false);
            }
            finally
            {
                if (page != null)
                {
                    try { await page.CloseAsync().ConfigureAwait(false); } catch { }
                }
                _pageLimiter?.Release();
            }
        }

        public async Task DisposeAsync()
        {
            try { if (_context != null) await _context.CloseAsync().ConfigureAwait(false); } catch { }
            try { if (_browser != null) await _browser.CloseAsync().ConfigureAwait(false); } catch { }
            try { _playwright?.Dispose(); } catch { }

            _context = null;
            _browser = null;
            _playwright = null;

            _pageLimiter?.Dispose();
            _pageLimiter = null;
        }

        public async ValueTask DisposeAsyncCore() => await DisposeAsync().ConfigureAwait(false);

        ValueTask IAsyncDisposable.DisposeAsync() => DisposeAsyncCore();
    }
}
