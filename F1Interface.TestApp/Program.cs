using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PlaywrightSharp;

namespace F1Interface.TestApp
{
    class Program
    {
        private static ContentService contentService;
        private static AuthenticationService authService;
        static async Task Main(string[] args)
        {
            Mock<ILogger<ContentService>> loggerMock = new Mock<ILogger<ContentService>>();
            Mock<ILogger<AuthenticationService>> authMock = new Mock<ILogger<AuthenticationService>>();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.69 Safari/537.36 Edg/89.0.774.39");
            client.DefaultRequestHeaders.TryAddWithoutValidation("origin", "https://f1tv.formula1.com");

            contentService = new ContentService(loggerMock.Object, client);

            await Playwright.InstallAsync();
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync();
            authService = new AuthenticationService(authMock.Object, browser);

            // Testing
            DoTests()
                .Wait();
            
            Console.ReadLine();
        }

        public static async Task DoTests()
        {
            await contentService.GetSeasonAsync(2018);
            await contentService.GetSeasonAsync(2020);
            await contentService.GetSeasonAsync(2021);
            await contentService.GetCurrentSeasonAsync();
            await contentService.GetUpcomingEventsAsync();
            await contentService.GetPastEventsAsync();

            await contentService.GetEventAsync(1043);

            await contentService.GetContentAsync(1000000823);
        }
    }
}
