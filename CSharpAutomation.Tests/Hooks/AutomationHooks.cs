using CSharpAutomation.Tests.Drivers;
using CSharpAutomation.Tests.Utils;
using Reqnroll;
using System.Text;
using System.Runtime.InteropServices;

namespace CSharpAutomation.Tests.Hooks
{
    [Binding]
    public class AutomationHooks
    {
        private readonly DriverFactory _driverFactory;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        
        // Static fields to store the report directory paths for the current test run
        private static string? _reportBaseDir;
        private static string? _allureResultsDir;
        private static string? _allureReportDir;
        private static DateTime _testRunStartTime;

        public AutomationHooks(DriverFactory driverFactory, ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _driverFactory = driverFactory;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
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
            
            // Create directories
            Directory.CreateDirectory(_allureResultsDir);
            
            // Clear the bin allure-results folder to prevent accumulation from previous runs
            var binAllureResultsDir = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");
            if (Directory.Exists(binAllureResultsDir))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(binAllureResultsDir))
                    {
                        File.Delete(file);
                    }
                    Logger.Info("Cleared previous allure results from bin folder");
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Could not clear bin allure-results: {ex.Message}");
                }
            }
            
            // Set environment variable for Allure to use
            Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY", _allureResultsDir);
            
            // Create environment.properties file for Allure report
            CreateEnvironmentProperties(_allureResultsDir);
            
            Logger.Info($"Report directory: {_reportBaseDir}");
            Logger.Info($"Allure results: {_allureResultsDir}");
            Logger.Separator();
        }
        
        private static void CreateEnvironmentProperties(string allureResultsDir)
        {
            try
            {
                var envFilePath = Path.Combine(allureResultsDir, "environment.properties");
                var envContent = new StringBuilder();
                
                // Add environment details
                envContent.AppendLine($"Browser=Chromium");
                envContent.AppendLine($"Browser.Headless=false");
                envContent.AppendLine($"OS={Environment.OSVersion}");
                envContent.AppendLine($"OS.Architecture={RuntimeInformation.OSArchitecture}");
                envContent.AppendLine($".NET.Version={Environment.Version}");
                envContent.AppendLine($"Machine.Name={Environment.MachineName}");
                envContent.AppendLine($"User.Name={Environment.UserName}");
                envContent.AppendLine($"Test.Environment=QA");
                envContent.AppendLine($"Base.URL=https://hotelbooker.cert.sabre.com");
                envContent.AppendLine($"Test.Run.Date={DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                envContent.AppendLine($"Framework=Reqnroll + NUnit + Playwright");
                
                File.WriteAllText(envFilePath, envContent.ToString());
                Logger.Debug("Created environment.properties file");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to create environment.properties", ex);
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Logger.Separator();
            Logger.Header("TEST RUN COMPLETED");
            
            // Generate Allure report after all tests complete
            try
            {
                // Allure.Reqnroll writes to bin folder by default, copy to our custom location
                var binAllureResultsDir = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");
                
                if (Directory.Exists(binAllureResultsDir) && !string.IsNullOrEmpty(_allureResultsDir))
                {
                    // Copy all files from bin allure-results to our custom location
                    var fileCount = 0;
                    foreach (var file in Directory.GetFiles(binAllureResultsDir))
                    {
                        var destFile = Path.Combine(_allureResultsDir, Path.GetFileName(file));
                        File.Copy(file, destFile, overwrite: true);
                        fileCount++;
                    }
                    Logger.Success($"Copied {fileCount} allure result files to: {_allureResultsDir}");
                }
                
                if (string.IsNullOrEmpty(_allureResultsDir) || !Directory.Exists(_allureResultsDir) || 
                    Directory.GetFiles(_allureResultsDir).Length == 0)
                {
                    Logger.Warn("Allure results directory is empty or not found");
                    return;
                }
                
                // Try to find allure executable - check common locations
                var allureExe = FindAllureExecutable();
                
                if (string.IsNullOrEmpty(allureExe))
                {
                    Logger.Error("Allure CLI not found. Please install Allure and add it to PATH.");
                    Logger.Info("Install via: choco install allure -y (run as admin)");
                    return;
                }
                
                Logger.Info("Generating Allure report...");
                
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = allureExe,
                    Arguments = $"generate \"{_allureResultsDir}\" -o \"{_allureReportDir}\" --clean",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using var process = System.Diagnostics.Process.Start(processInfo);
                if (process != null)
                {
                    process.WaitForExit(60000); // Wait up to 60 seconds
                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    
                    if (process.ExitCode == 0)
                    {
                        Logger.Success($"Allure report generated at: {_allureReportDir}");
                        
                        // Automatically open the Allure report in browser
                        OpenAllureReport(allureExe, _allureReportDir!);
                    }
                    else
                    {
                        Logger.Error($"Allure report generation failed: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error generating Allure report", ex);
                Logger.Info("Make sure 'allure' CLI is installed and available in PATH.");
            }
        }
        
        private static void OpenAllureReport(string allureExe, string reportDir)
        {
            try
            {
                Logger.Info("Opening Allure report in browser...");
                
                var openProcessInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = allureExe,
                    Arguments = $"open \"{reportDir}\"",
                    UseShellExecute = false,
                    CreateNoWindow = false
                };
                
                // Start the process but don't wait for it (it runs a web server)
                System.Diagnostics.Process.Start(openProcessInfo);
                
                Logger.Success("Allure report server started. Press Ctrl+C in the server window to stop.");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to open Allure report", ex);
                Logger.Info($"You can manually open: {reportDir}");
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
        
        private static string? FindAllureExecutable()
        {
            // Check common installation locations
            var possiblePaths = new[]
            {
                @"C:\tools\allure\bin\allure.bat",
                @"C:\ProgramData\chocolatey\bin\allure.bat",
                @"C:\Program Files\allure\bin\allure.bat",
                "allure" // Try PATH
            };
            
            foreach (var path in possiblePaths)
            {
                if (path == "allure")
                {
                    // Check if allure is in PATH
                    try
                    {
                        var psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "where",
                            Arguments = "allure",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using var proc = System.Diagnostics.Process.Start(psi);
                        if (proc != null)
                        {
                            proc.WaitForExit(5000);
                            if (proc.ExitCode == 0)
                            {
                                return "allure";
                            }
                        }
                    }
                    catch { }
                }
                else if (File.Exists(path))
                {
                    return path;
                }
            }
            
            return null;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Logger.Step($"Scenario: {_scenarioContext.ScenarioInfo.Title}");
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
