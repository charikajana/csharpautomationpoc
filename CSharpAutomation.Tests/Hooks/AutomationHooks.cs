using CSharpAutomation.Tests.Drivers;
using CSharpAutomation.Tests.Utils;
using Reqnroll;
using Microsoft.Extensions.Configuration;

namespace CSharpAutomation.Tests.Hooks
{
    [Binding]
    public class AutomationHooks
    {
        private readonly DriverFactory _driverFactory;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly IConfiguration _configuration;
        
        // Static fields to store the report directory paths for the current test run
        private static string? _reportBaseDir;
        private static string? _allureResultsDir;
        private static string? _allureReportDir;
        private static DateTime _testRunStartTime;

        public AutomationHooks(DriverFactory driverFactory, ScenarioContext scenarioContext, FeatureContext featureContext, IConfiguration configuration)
        {
            _driverFactory = driverFactory;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            _configuration = configuration;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Logger.Header("TEST RUN STARTED");
            
            // Capture the test run start time
            _testRunStartTime = DateTime.Now;
            
            // Get the project root directory (go up from bin/Debug/net10.0)
            var projectDir = GetProjectDirectory();
            
            // Create timestamped folder structure: reports/19DEC/1552/
            var dateFolder = _testRunStartTime.ToString("ddMMM").ToUpper(); // e.g., 19DEC
            var timeFolder = _testRunStartTime.ToString("HHmm"); // e.g., 1552
            
            _reportBaseDir = Path.Combine(projectDir, "reports", dateFolder, timeFolder);
            _allureResultsDir = Path.Combine(_reportBaseDir, "allure-results");
            _allureReportDir = Path.Combine(_reportBaseDir, "allure-report");
            
            // Configure Allure Environment using Helper
            AllureHelper.ConfigureAllureEnvironment(_allureResultsDir);
            
            Logger.Info($"Report directory: {_reportBaseDir}");
            Logger.Separator();
        }
        
        [AfterTestRun]
        public static void AfterTestRun()
        {
            Logger.Separator();
            Logger.Header("TEST RUN COMPLETED");
            
            // Generate Allure report using Helper
            if (!string.IsNullOrEmpty(_allureResultsDir) && !string.IsNullOrEmpty(_allureReportDir))
            {
                AllureHelper.GenerateAllureReport(_allureResultsDir, _allureReportDir);
            }
        }
        
        private static string GetProjectDirectory()
        {
            // Get the current directory (typically bin/Debug/net10.0 when running tests)
            var currentDir = Directory.GetCurrentDirectory();
            
            // Try to find the project directory by looking for .csproj file
            var dir = new DirectoryInfo(currentDir);
            while (dir != null)
            {
                var csprojFiles = dir.GetFiles("*.csproj");
                if (csprojFiles.Length > 0)
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            
            // Fallback: go up 3 levels from bin/Debug/net10.0
            return Path.GetFullPath(Path.Combine(currentDir, "..", "..", ".."));
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Logger.Step($"Scenario: {_scenarioContext.ScenarioInfo.Title}");

            // Apply Allure Tags using Helper
            AllureHelper.ApplyScenarioTags(_scenarioContext, _configuration);
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            // Take screenshot on failure and save to reports directory
            if (_scenarioContext.TestError != null)
            {
                Logger.Error($"Scenario FAILED: {_scenarioContext.TestError.Message}");
                
                try
                {
                    var screenshotDir = Path.Combine(_reportBaseDir ?? Directory.GetCurrentDirectory(), "Screenshots");
                    if (!Directory.Exists(screenshotDir))
                    {
                        Directory.CreateDirectory(screenshotDir);
                    }
                    
                    var screenshotPath = Path.Combine(screenshotDir, $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    
                    await _driverFactory.Page.ScreenshotAsync(new Microsoft.Playwright.PageScreenshotOptions
                    {
                        Path = screenshotPath,
                        FullPage = true
                    });
                    
                    Logger.Info($"Failure screenshot saved: {screenshotPath}");
                }
                catch
                {
                    // Page might already be closed
                    Logger.Warn("Could not capture failure screenshot - page may be closed");
                }
            }
            else
            {
                Logger.Success($"Scenario PASSED: {_scenarioContext.ScenarioInfo.Title}");
            }
        }
    }
}
