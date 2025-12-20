using Allure.Net.Commons;
using Microsoft.Extensions.Configuration;
using Reqnroll;
using System.Runtime.InteropServices;
using System.Text;

namespace CSharpAutomation.Tests.Utils
{
    public static class AllureHelper
    {
        public static void ConfigureAllureEnvironment(string allureResultsDir)
        {
            // Create directories
            Directory.CreateDirectory(allureResultsDir);

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
            Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY", allureResultsDir);

            // Create environment.properties file for Allure report
            CreateEnvironmentProperties(allureResultsDir);
            
            Logger.Info($"Allure results configured at: {allureResultsDir}");
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

        public static void GenerateAllureReport(string allureResultsDir, string allureReportDir)
        {
            try
            {
                // Allure.Reqnroll writes to bin folder by default, copy to our custom location if needed
                var binAllureResultsDir = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");

                if (Directory.Exists(binAllureResultsDir) && !string.IsNullOrEmpty(allureResultsDir))
                {
                    // If they are different directories, copy files
                    if (Path.GetFullPath(binAllureResultsDir) != Path.GetFullPath(allureResultsDir))
                    {
                        var fileCount = 0;
                        foreach (var file in Directory.GetFiles(binAllureResultsDir))
                        {
                            var destFile = Path.Combine(allureResultsDir, Path.GetFileName(file));
                            File.Copy(file, destFile, overwrite: true);
                            fileCount++;
                        }
                        Logger.Success($"Copied {fileCount} allure result files to: {allureResultsDir}");
                    }
                }

                if (string.IsNullOrEmpty(allureResultsDir) || !Directory.Exists(allureResultsDir) ||
                    Directory.GetFiles(allureResultsDir).Length == 0)
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
                    Arguments = $"generate \"{allureResultsDir}\" -o \"{allureReportDir}\" --clean",
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
                        Logger.Success($"Allure report generated at: {allureReportDir}");

                        // Automatically open the Allure report in browser
                        OpenAllureReport(allureExe, allureReportDir);
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

        public static void ApplyScenarioTags(ScenarioContext scenarioContext, IConfiguration configuration)
        {
            string IssueBaseUrl = configuration["Links:Issue"] ?? "https://github.com/your-org/your-repo/issues/";
            string TmsBaseUrl = configuration["Links:Tms"] ?? "https://jira.yourcompany.com/browse/";
            string GenericLinkBaseUrl = configuration["Links:Generic"] ?? "https://yourcompany.atlassian.net/browse/";

            foreach (var tag in scenarioContext.ScenarioInfo.Tags)
            {
                if (tag.StartsWith("issue:"))
                {
                    var id = tag.Substring("issue:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.links.Add(new Link { name = id, type = "issue", url = IssueBaseUrl + id });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
                else if (tag.StartsWith("tms:"))
                {
                    var id = tag.Substring("tms:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.links.Add(new Link { name = id, type = "tms", url = TmsBaseUrl + id });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
                else if (tag.StartsWith("link:"))
                {
                    var id = tag.Substring("link:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.links.Add(new Link { name = id, type = "link", url = GenericLinkBaseUrl + id });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
                else if (tag.StartsWith("epic:"))
                {
                    var value = tag.Substring("epic:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.labels.Add(new Label { name = "epic", value = value });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
                else if (tag.StartsWith("feature:"))
                {
                    var value = tag.Substring("feature:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.labels.Add(new Label { name = "feature", value = value });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
                else if (tag.StartsWith("story:"))
                {
                    var value = tag.Substring("story:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.labels.Add(new Label { name = "story", value = value });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
                else if (tag.StartsWith("owner:"))
                {
                    var value = tag.Substring("owner:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.labels.Add(new Label { name = "owner", value = value });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
                else if (tag.StartsWith("severity:"))
                {
                    var value = tag.Substring("severity:".Length);
                    AllureLifecycle.Instance.UpdateTestCase(t =>
                    {
                        t.labels.Add(new Label { name = "severity", value = value.ToLower() });
                        t.labels.RemoveAll(l => l.name == "tag" && l.value == tag);
                    });
                }
            }
        }
    }
}
