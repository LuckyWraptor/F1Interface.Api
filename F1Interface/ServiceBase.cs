using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlaywrightSharp;

namespace F1Interface
{
    public abstract class ServiceBase<T>
    {
        private const bool EmulateRealDelays = true;
        protected static readonly Random random = new Random();

        protected readonly ILogger<T> logger;
        protected readonly IBrowser browser;

        internal ServiceBase(ILogger<T> logger, IBrowser browser)
        {
            if (browser == null)
            {
                throw new ArgumentNullException("Provided IApiBrowser cannot be null!");
            }

            this.browser = browser;
            this.logger = logger;
        }

        /// <summary>
        /// Create a random task delay
        /// </summary>
        /// <param name="cancellationToken">Taks cancellation request token</param>
        protected Task RandomDelay(CancellationToken cancellationToken)
            => RandomDelay(200, 1500, cancellationToken);
        /// <summary>
        /// Create a random task delay
        /// </summary>
        /// <param name="minimum">Minimum amount of milliseconds to wait</param>
        /// <param name="maximum">Maximum amount of milliseconds to wait</param>
        /// <param name="cancellationToken">Taks cancellation request token</param>
        protected Task RandomDelay(int minimum, int maximum, CancellationToken cancellationToken)
            => Task.Delay((EmulateRealDelays ? random.Next(minimum, maximum) : 0),
                cancellationToken);
        /// <summary>
        /// Create a new page using a random user-agent and the preconfigured viewport
        /// </summary>
        /// <param name="browser">\_(^_^)_/</param>
        /// <param name="random">Random generator to use, defaults to a newly created one</param>
        /// <returns>The created page</returns>
        protected Task<IPage> NewPageAsync()
            => browser.NewPageAsync(new BrowserContextOptions
            {
                Viewport = new ViewportSize { Height = 979, Width = 1920 },
                UserAgent = "",
                AcceptDownloads = false
            });
        /// <summary>
        /// Click a button on the provided page
        /// </summary>
        /// <param name="page">Page to click in</param>
        /// <param name="selector">Document element selector to click the correct button/field</param>
        protected Task ClickButtonAsync(IPage page, string selector)
            => page.ClickAsync(selector, (EmulateRealDelays ? random.Next(50, 250) : 0));

        protected async Task TypeAsync(IPage page, string selector, string text, CancellationToken cancellationToken)
        {
            IElementHandle handle = await page.QuerySelectorAsync(selector)
                .ConfigureAwait(false);

            if (handle != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    await RandomDelay(15, 100, cancellationToken);
                    await handle.PressAsync(text[i].ToString())
                        .ConfigureAwait(false);
                }
            }
        }
    }
}