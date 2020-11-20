using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlaywrightSharp;

namespace F1Interface.Api.Util
{
    public class BrowserUtils
    {
        internal static readonly string[] UserAgents = new string[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 11_0_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36"
        };
        internal static readonly ViewportSize ViewportSize = new ViewportSize { Height = 969, Width = 1920 };

        /// <summary>
        /// Create a new page using a random user-agent and the preconfigured viewport
        /// </summary>
        /// <param name="browser">\_(^_^)_/</param>
        /// <param name="random">Random generator to use, defaults to a newly created one</param>
        /// <returns>The created page</returns>
        public static Task<IPage> NewPageAsync(IBrowser browser, Random random = default)
        {
            string userAgent = UserAgents[
                (random ?? new Random()).Next(UserAgents.Length)];

            return browser.NewPageAsync(new BrowserContextOptions
            {
                Viewport = ViewportSize,
                UserAgent = userAgent,
                AcceptDownloads = false
            });
        }
    }
}
