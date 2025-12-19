using Microsoft.Playwright;
using Microsoft.Extensions.Configuration;

namespace CSharpAutomation.Tests.Drivers
{
    public class DriverFactory : IDisposable
    {
        private readonly Task<IPage> _page;
        private IBrowser? _browser;
        private IPlaywright? _playwright;

        public DriverFactory(IConfiguration configuration)
        {
            _page = Task.Run(async () =>
            {
                _playwright = await Playwright.CreateAsync();
                
                var browserType = configuration["Browser"] ?? "chromium";
                var headless = bool.Parse(configuration["Headless"] ?? "false");
                var slowMo = int.Parse(configuration["SlowMo"] ?? "0");

                var launchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    SlowMo = slowMo
                };

                _browser = browserType.ToLower() switch
                {
                    "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),
                    "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),
                    _ => await _playwright.Chromium.LaunchAsync(launchOptions)
                };

                var context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 1580, Height = 780 }
                });

                return await context.NewPageAsync();
            });
        }

        public IPage Page => _page.Result;

        public void Dispose()
        {
            _browser?.CloseAsync().Wait();
            _playwright?.Dispose();
        }
    }
}
