using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace FoodManager
{
    // Simple shared Playwright pool: single browser+context, limited concurrent pages.
    public static class PlaywrightPool
    {
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;
        private static IBrowserContext? _context;
        private static SemaphoreSlim? _semaphore;
        private static bool _initialized = false;

        // Initialize pool: call once before doing many fetches.
        public static async Task InitializeAsync(int maxParallel = 4)
        {
            if (_initialized) return;
            _playwright = await Playwright.CreateAsync();
            // you can tune args here (e.g. disable sandbox on linux, etc.)
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
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                Locale = "uk-UA",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
            });

            _semaphore = new SemaphoreSlim(maxParallel, maxParallel);
            _initialized = true;
        }

        // Dispose browser/context
        public static async Task DisposeAsync()
        {
            try
            {
                if (_context != null) await _context.CloseAsync();
            }
            catch { }
            try
            {
                if (_browser != null) await _browser.CloseAsync();
            }
            catch { }
            try
            {
                _playwright?.Dispose();
            }
            catch { }

            _context = null;
            _browser = null;
            _playwright = null;
            _semaphore?.Dispose();
            _semaphore = null;
            _initialized = false;
        }

        // Run a function with a fresh page created from shared context.
        // The function receives an IPage and returns T.
        // Pool limits concurrent pages to maxParallel.
        public static async Task<T> RunWithPageAsync<T>(Func<IPage, Task<T>> func, CancellationToken ct)
        {
            if (!_initialized) throw new InvalidOperationException("PlaywrightPool not initialized. Call InitializeAsync first.");

            if (_semaphore == null) throw new InvalidOperationException("Semaphore not initialized.");

            await _semaphore.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                ct.ThrowIfCancellationRequested();
                var page = await _context!.NewPageAsync().ConfigureAwait(false);

                // Block unnecessary resources (images, fonts, media) to speed up loading.
                // Keep scripts/styles because some sites render prices via JS; if you know static pages,
                // you can block more to speed up further.
                await page.RouteAsync("**/*", async route =>
                {
                    try
                    {
                        var request = route.Request;
                        var resType = request.ResourceType;
                        if (resType == "image" || resType == "font" || resType == "media")
                        {
                            await route.AbortAsync();
                        }
                        else
                        {
                            await route.ContinueAsync();
                        }
                    }
                    catch
                    {
                        try { await route.ContinueAsync(); } catch { }
                    }
                }).ConfigureAwait(false);

                try
                {
                    var result = await func(page).ConfigureAwait(false);
                    // close page to free memory
                    await page.CloseAsync().ConfigureAwait(false);
                    return result;
                }
                catch
                {
                    try { await page.CloseAsync().ConfigureAwait(false); } catch { }
                    throw;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}