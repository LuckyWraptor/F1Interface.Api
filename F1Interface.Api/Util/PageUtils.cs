using System;
using System.Threading;
using System.Threading.Tasks;

using PlaywrightSharp;

namespace F1Interface.Api.Util
{
    public class PageUtils
    {
        /// <summary>
        /// Click a target using a random release delay
        /// </summary>
        /// <param name="page">"\_(^_^)_/"</param>
        /// <param name="selector">Query selector</param>
        /// <param name="random">Random generator</param>
        public static async Task ClickAsync(IPage page, string selector, Random random = default)
        {
            await page.ClickAsync(selector, TaskUtils.GetRandomDelay(random, 50, 300))
                        .ConfigureAwait(false);
        }
        /// <summary>
        /// Type a text using random intervals
        /// </summary>
        /// <param name="page">"\_(^_^)_/"</param>
        /// <param name="selector">Query selector</param>
        /// <param name="text">Text to write down</param>
        /// <param name="minimumDelay">Minimum type interval</param>
        /// <param name="maximumDelay">Maximum type interval</param>
        /// <param name="random">Random generator</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task TypeAsync(IPage page, string selector, string text, int minimumDelay = 50, int maximumDelay = 300,
            Random random = default, CancellationToken cancellationToken = default)
        {
            IElementHandle handle = await page.QuerySelectorAsync(selector)
                        .ConfigureAwait(false);

            if (handle == null)
                return;

            if (random == null)
                random = new Random();

            for (int i = 0; i < text.Length; i++)
            {
                await TaskUtils.RandomDelay(random, minimumDelay, maximumDelay, cancellationToken)
                            .ConfigureAwait(false);
                await handle.PressAsync(text[i].ToString(),
                        TaskUtils.GetRandomDelay(random, (minimumDelay / 2), minimumDelay))
                            .ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Ignore unnecessary content like images & fonts
        /// </summary>
        /// <param name="page">Page to ignore from.</param>
        public static async Task IgnoreExtraContent(IPage page)
        {
            await page.RouteAsync("**/*.{png,jpg,svg,ico,woff2}", (route, _) => route.AbortAsync())
                        .ConfigureAwait(false);
        }
        /// <summary>
        /// Restore unnecessary content like images & fonts
        /// </summary>
        /// <param name="page">Page to unignore from.</param>
        public static async Task UnignoreExtraContent(IPage page)
        {
            await page.UnrouteAsync("**/*.{png,jpg,svg,ico,woff2}")
                        .ConfigureAwait(false);
        }
    }
}
