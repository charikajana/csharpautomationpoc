using CSharpAutomation.Tests.Drivers;
using CSharpAutomation.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using Reqnroll;

namespace CSharpAutomation.Tests.Support
{
    public static class DependencyInjection
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            // Get environment from environment variable, default to "DEV"
            var environment = Environment.GetEnvironmentVariable("TEST_ENV") ?? "DEV";
            
            Logger.Info($"Loading configuration for environment: {environment}");

            // Build paths
            var basePath = Directory.GetCurrentDirectory();
            var envConfigPath = Path.Combine(basePath, "Environments", $"appsettings.{environment}.json");

            // Load configuration with environment-specific override from Environments folder
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add environment-specific config if exists
            if (File.Exists(envConfigPath))
            {
                configBuilder.AddJsonFile(envConfigPath, optional: true, reloadOnChange: true);
                Logger.Info($"Loaded environment config: Environments/appsettings.{environment}.json");
            }
            else
            {
                Logger.Warn($"Environment config not found: {envConfigPath}");
            }

            var configuration = configBuilder.Build();

            // Log the loaded configuration
            Logger.Info($"Environment: {configuration["Environment"]}");
            Logger.Info($"Base URL: {configuration["BaseUrl"]}");
            Logger.Info($"Browser: {configuration["Browser"]}");
            Logger.Info($"Headless: {configuration["Headless"]}");

            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<DriverFactory>();

            return services;
        }
    }
}
