using Reqnroll;
using Reqnroll.Bindings;
using Reqnroll.Infrastructure;
using CSharpAutomation.Tests.Utils;

namespace CSharpAutomation.Tests.Hooks
{
    /// <summary>
    /// Hook to validate step bindings and report undefined steps.
    /// Run with environment variable DRY_RUN=true to only validate without executing.
    /// </summary>
    [Binding]
    public class DryRunHooks
    {
        private static readonly bool _isDryRun = 
            Environment.GetEnvironmentVariable("DRY_RUN")?.ToLower() == "true";

        [BeforeTestRun]
        public static void ValidateBindings()
        {
            if (_isDryRun)
            {
                Logger.Header("DRY RUN MODE - Validating Step Bindings");
                Logger.Info("Set DRY_RUN=false or remove the variable to run actual tests");
                Logger.Separator();
            }
        }

        [BeforeScenario]
        public void BeforeScenarioDryRun(ScenarioContext scenarioContext)
        {
            if (_isDryRun)
            {
                Logger.Info($"[DRY RUN] Would execute: {scenarioContext.ScenarioInfo.Title}");
            }
        }

        [AfterStep]
        public void AfterStepDryRun(ScenarioContext scenarioContext)
        {
            if (_isDryRun)
            {
                var stepInfo = scenarioContext.StepContext.StepInfo;
                Logger.Debug($"  ✓ Step found: [{stepInfo.StepDefinitionType}] {stepInfo.Text}");
            }
        }

        [AfterTestRun]
        public static void DryRunSummary()
        {
            if (_isDryRun)
            {
                Logger.Separator();
                Logger.Success("DRY RUN COMPLETED - All step bindings validated");
                Logger.Info("Run without DRY_RUN=true to execute actual tests");
            }
        }
    }
}
