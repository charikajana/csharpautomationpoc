# C# Playwright Automation Framework

A comprehensive, production-grade test automation framework built with **C#**, **Playwright**, **Reqnroll (BDD)**, **NUnit**, and **Allure Reporting**.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Running Tests](#running-tests)
- [Allure Reporting](#allure-reporting)
- [BasePage Methods Reference](#basepage-methods-reference)
- [Creating New Tests](#creating-new-tests)
- [Tags and Filtering](#tags-and-filtering)
- [Configuration](#configuration)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

---

## 🎯 Overview

This framework provides a robust, scalable, and maintainable solution for browser automation testing. It follows the **Page Object Model (POM)** design pattern and uses **Behavior-Driven Development (BDD)** with Gherkin syntax.

### Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Runtime |
| Playwright | 1.47.0 | Browser Automation |
| Reqnroll | 3.0.3 | BDD Framework |
| NUnit | 3.14.0 | Test Framework |
| Allure | 2.14.1 | Reporting |

---

## ✨ Features

- ✅ **Cross-browser testing** (Chromium, Firefox, WebKit)
- ✅ **BDD with Gherkin syntax** (Feature files)
- ✅ **Auto-generated Allure reports** with screenshots
- ✅ **Page Object Model** architecture
- ✅ **Comprehensive BasePage** with 100+ reusable methods
- ✅ **Automatic screenshot capture** on test failure
- ✅ **Environment details** in reports
- ✅ **Tag-based test filtering**
- ✅ **Timestamped report folders**
- ✅ **Dependency Injection** support

---

## 📦 Prerequisites

1. **.NET SDK 10.0+**
   ```powershell
   # Verify installation
   dotnet --version
   ```

2. **Allure CLI** (for report generation)
   ```powershell
   # Install via Chocolatey
   choco install allure -y
   
   # Or download from: https://github.com/allure-framework/allure2/releases
   ```

3. **Java JDK 11+** (required for Allure)
   ```powershell
   java --version
   ```

---

## 📁 Project Structure

```
CSharpAutomation.Tests/
├── 📁 bin/                          # Build output
├── 📁 Drivers/
│   └── DriverFactory.cs             # Browser driver initialization
├── 📁 Features/
│   ├── Login.feature                # Gherkin feature files
│   └── 📁 Newfeatures/
│       └── basic.feature
├── 📁 Hooks/
│   └── AutomationHooks.cs           # Test lifecycle hooks
├── 📁 Pages/
│   ├── BasePage.cs                  # Base page with reusable methods
│   └── LoginPage.cs                 # Page object example
├── 📁 reports/                      # Generated reports
│   └── 📁 {ddMMM}/                  # Date folder (e.g., 19DEC)
│       └── 📁 {HHmm}/               # Time folder (e.g., 1655)
│           ├── 📁 allure-results/
│           ├── 📁 allure-report/
│           └── 📁 Screenshots/
├── 📁 Steps/
│   └── LoginSteps.cs                # Step definitions
├── 📁 Support/
│   └── DependencyInjection.cs       # DI configuration
├── allureConfig.json                # Allure configuration
├── appsettings.json                 # App configuration
└── CSharpAutomation.Tests.csproj    # Project file
```

---

## 🚀 Setup Instructions

### 1. Clone/Create Project

```powershell
# Navigate to project directory
cd C:\path\to\CSharpAutomation\CSharpAutomation.Tests
```

### 2. Restore Packages

```powershell
dotnet restore
```

### 3. Build Project

```powershell
dotnet build
```

### 4. Install Playwright Browsers

```powershell
# Navigate to bin folder and run Playwright install
dotnet exec bin/Debug/net10.0/Microsoft.Playwright.dll install
```

---

## 🧪 Running Tests

### Run All Tests

```powershell
dotnet test
```

### Run Tests with Specific Tag

```powershell
# Run only @sanity tests
dotnet test --filter "Category=sanity"

# Run only @Regression tests
dotnet test --filter "Category=Regression"

# Run only @LoginTest tests
dotnet test --filter "Category=LoginTest"
```

### Run Tests with Multiple Tags

```powershell
# OR condition (run either)
dotnet test --filter "Category=sanity|Category=smoke"

# AND condition (run tests with both tags)
dotnet test --filter "Category=sanity&Category=LoginTest"
```

### Run with Verbose Output

```powershell
dotnet test --verbosity detailed
```

---

## 📊 Allure Reporting

### Report Location

Reports are automatically generated in timestamped folders:

```
reports/{ddMMM}/{HHmm}/
├── allure-results/      # Raw test data
├── allure-report/       # HTML report
└── Screenshots/         # Test screenshots
```

Example: `reports/19DEC/1655/allure-report/`

### View Report

After test execution, the report opens automatically. If not:

```powershell
# Open existing report
C:\tools\allure\bin\allure.bat open "path\to\allure-report"

# Or serve from results
C:\tools\allure\bin\allure.bat serve "path\to\allure-results"
```

### Report Features

- ✅ Test execution summary
- ✅ Pass/Fail statistics with graphs
- ✅ Step-by-step execution details
- ✅ Screenshots attached to steps
- ✅ Environment information
- ✅ Test history and trends
- ✅ Categories and suites view

### Environment Information

The report includes:
- Browser name and mode
- Operating System
- .NET Version
- Machine name
- Test environment
- Base URL
- Test run date/time
- Framework details

---

## 📚 BasePage Methods Reference

The `BasePage` class provides **100+ reusable methods** organized into categories:

### Navigation Methods

```csharp
await NavigateAsync("https://example.com");
await NavigateAsync("https://example.com", WaitUntilState.NetworkIdle);
await GoBackAsync();
await GoForwardAsync();
await ReloadAsync();
string url = GetCurrentUrl();
string title = await GetTitleAsync();
```

### Click Methods

```csharp
await ClickAsync("#button");
await DoubleClickAsync("#element");
await RightClickAsync("#element");
await ForceClickAsync("#element");      // Bypasses actionability checks
await JsClickAsync("#element");         // JavaScript click
```

### Input Methods

```csharp
await FillAsync("#input", "text");      // Clear and fill
await TypeAsync("#input", "text", 50);  // Type character by character
await ClearAsync("#input");
await PressKeyAsync("Enter");
await PressKeyAsync("#input", "Tab");
await SetValueAsync("#input", "value"); // JavaScript set value
```

### Get Text/Attribute Methods

```csharp
string? text = await GetTextAsync("#element");
string innerText = await GetInnerTextAsync("#element");
string html = await GetInnerHtmlAsync("#element");
string? attr = await GetAttributeAsync("#element", "class");
string value = await GetInputValueAsync("#input");
IReadOnlyList<string> texts = await GetAllTextsAsync(".items");
int count = await GetElementCountAsync(".items");
```

### Element State Methods

```csharp
bool isVisible = await IsVisibleAsync("#element");
bool isHidden = await IsHiddenAsync("#element");
bool isEnabled = await IsEnabledAsync("#button");
bool isDisabled = await IsDisabledAsync("#button");
bool isChecked = await IsCheckedAsync("#checkbox");
bool isEditable = await IsEditableAsync("#input");
bool exists = await ElementExistsAsync("#element");
```

### Wait Methods

```csharp
await WaitForVisibleAsync("#element", 30000);
await WaitForHiddenAsync("#loader");
await WaitForAttachedAsync("#element");
await WaitForDetachedAsync("#element");
await WaitForLoadStateAsync(LoadState.NetworkIdle);
await WaitForUrlAsync("dashboard");
await WaitAsync(2000);                   // Static wait (use sparingly)
await WaitForResponseAsync("/api/data");
await WaitForNetworkIdleAsync();
```

### Select/Dropdown Methods

```csharp
await SelectByValueAsync("#dropdown", "option1");
await SelectByTextAsync("#dropdown", "Option Text");
await SelectByIndexAsync("#dropdown", 2);
await SelectMultipleAsync("#dropdown", new[] { "val1", "val2" });
var options = await GetSelectOptionsAsync("#dropdown");
string selected = await GetSelectedOptionTextAsync("#dropdown");
```

### Checkbox/Radio Methods

```csharp
await CheckAsync("#checkbox");
await UncheckAsync("#checkbox");
await SetCheckedAsync("#checkbox", true);
```

### Hover/Focus Methods

```csharp
await HoverAsync("#element");
await FocusAsync("#input");
await BlurAsync("#input");
```

### Scroll Methods

```csharp
await ScrollIntoViewAsync("#element");
await ScrollToTopAsync();
await ScrollToBottomAsync();
await ScrollByAsync(0, 500);
await ScrollToAsync(0, 1000);
```

### Frame Methods

```csharp
IFrame? frame = GetFrame("frameName");
IFrameLocator frameLocator = GetFrameLocator("#iframe");
var allFrames = GetAllFrames();
```

### Alert/Dialog Methods

```csharp
SetupAutoAcceptDialogs();
SetupAutoDismissDialogs();
string message = await AcceptNextDialogAsync(() => ClickAsync("#alertBtn"));
string message = await DismissNextDialogAsync(() => ClickAsync("#confirmBtn"));
string message = await HandlePromptDialogAsync(() => ClickAsync("#promptBtn"), "input text");
var (type, message) = await GetNextDialogInfoAsync(() => ClickAsync("#btn"));
```

### File Upload Methods

```csharp
await UploadFileAsync("#fileInput", @"C:\path\to\file.pdf");
await UploadFilesAsync("#fileInput", new[] { "file1.pdf", "file2.pdf" });
await ClearFileInputAsync("#fileInput");
```

### Drag and Drop

```csharp
await DragAndDropAsync("#source", "#target");
```

### JavaScript Execution

```csharp
var result = await ExecuteJsAsync<string>("return document.title");
await ExecuteJsAsync("alert('Hello')");
var text = await ExecuteJsOnElementAsync<string>("#el", "el => el.textContent");
```

### Screenshot Methods

```csharp
await TakeScreenshotAsync("StepName");              // Saves & attaches to Allure
byte[] bytes = await TakeElementScreenshotAsync("#element");
byte[] bytes = await TakeFullPageScreenshotAsync();
```

### Locator Methods

```csharp
ILocator locator = Locator("#element");
ILocator byRole = GetByRole(AriaRole.Button, new() { Name = "Submit" });
ILocator byText = GetByText("Click me");
ILocator byLabel = GetByLabel("Email");
ILocator byPlaceholder = GetByPlaceholder("Enter name");
ILocator byAltText = GetByAltText("Logo");
ILocator byTitle = GetByTitle("Submit Form");
ILocator byTestId = GetByTestId("submit-btn");
```

### Browser Context Methods

```csharp
// Cookies
var cookies = await GetCookiesAsync();
await AddCookiesAsync(cookiesList);
await ClearCookiesAsync();

// Local Storage
string? value = await GetLocalStorageItemAsync("key");
await SetLocalStorageItemAsync("key", "value");
await ClearLocalStorageAsync();

// Session Storage
string? value = await GetSessionStorageItemAsync("key");
await SetSessionStorageItemAsync("key", "value");
await ClearSessionStorageAsync();
```

### Keyboard Methods

```csharp
await PressEnterAsync();
await PressTabAsync();
await PressEscapeAsync();
await PressShortcutAsync("Control+A");
await KeyboardTypeAsync("text");
```

### Mouse Methods

```csharp
await MouseMoveAsync(100, 200);
await MouseClickAsync(100, 200);
await MouseDoubleClickAsync(100, 200);
await MouseWheelAsync(0, 500);
```

### Window/Tab Management

```csharp
var allPages = GetAllPages();
int pageCount = GetPageCount();
IPage newPage = await WaitForNewPageAsync(() => ClickAsync("#newTabLink"));
IPage page = SwitchToPageByIndex(1);
IPage? page = SwitchToPageByUrl("dashboard");
IPage? page = await SwitchToPageByTitleAsync("Dashboard");
IPage? remaining = await CloseCurrentPageAndSwitchAsync();
await ClosePageByIndexAsync(1);
await CloseOtherPagesAsync();
IPage newPage = await OpenNewPageAsync();
IPage newPage = await OpenNewPageAsync("https://example.com");
await BringToFrontAsync();
```

### Assertion Methods

```csharp
// Text Assertions
await AssertTextContainsAsync("#el", "expected");
await AssertTextEqualsAsync("#el", "exact text");

// Visibility Assertions
await AssertVisibleAsync("#el");
await AssertHiddenAsync("#el");

// State Assertions
await AssertEnabledAsync("#btn");
await AssertDisabledAsync("#btn");
await AssertCheckedAsync("#checkbox");
await AssertNotCheckedAsync("#checkbox");
await AssertExistsAsync("#el");
await AssertNotExistsAsync("#el");

// Attribute Assertions
await AssertAttributeEqualsAsync("#el", "class", "active");
await AssertAttributeContainsAsync("#el", "class", "btn");
await AssertHasClassAsync("#el", "active");

// Count Assertions
await AssertElementCountAsync(".items", 5);
await AssertElementCountGreaterThanAsync(".items", 0);

// URL/Title Assertions
AssertUrlContains("dashboard");
AssertUrlEquals("https://example.com/dashboard");
await AssertTitleContainsAsync("Dashboard");
await AssertTitleEqualsAsync("My Dashboard");

// Input Assertions
await AssertInputValueAsync("#input", "expected value");

// Custom Assertions
Assert(condition, "Error message");

// Playwright Built-in Expect
await Expect("#el").ToBeVisibleAsync();
await ExpectPage().ToHaveTitleAsync("Title");
```

### Soft Assertions

```csharp
// Collect failures without stopping test
await SoftAssertTextContainsAsync("#el", "text");
await SoftAssertVisibleAsync("#el");

// Get all failures
var errors = GetSoftAssertionErrors();

// Assert all at end of test (throws if any failed)
AssertAllSoftAssertions();

// Clear errors
ClearSoftAssertions();
```

---

## ✍️ Creating New Tests

### 1. Create Feature File

Create `Features/YourFeature.feature`:

```gherkin
@Regression
Feature: User Registration

@smoke
Scenario: Successful user registration
    Given I navigate to the registration page
    When I enter valid registration details
    And I submit the registration form
    Then I should see a success message
```

### 2. Create Page Object

Create `Pages/RegistrationPage.cs`:

```csharp
using Microsoft.Playwright;

namespace CSharpAutomation.Tests.Pages
{
    public class RegistrationPage : BasePage
    {
        public RegistrationPage(IPage page) : base(page) { }

        // Locators
        private string NameField => "#name";
        private string EmailField => "#email";
        private string PasswordField => "#password";
        private string SubmitButton => "#submit";
        private string SuccessMessage => ".success-msg";

        // Actions
        public async Task FillRegistrationFormAsync(string name, string email, string password)
        {
            await FillAsync(NameField, name);
            await FillAsync(EmailField, email);
            await FillAsync(PasswordField, password);
            await TakeScreenshotAsync("RegistrationForm_Filled");
        }

        public async Task SubmitFormAsync()
        {
            await ClickAsync(SubmitButton);
        }

        public async Task<string?> GetSuccessMessageAsync()
        {
            await WaitForVisibleAsync(SuccessMessage);
            return await GetTextAsync(SuccessMessage);
        }
    }
}
```

### 3. Create Step Definitions

Create `Steps/RegistrationSteps.cs`:

```csharp
using CSharpAutomation.Tests.Drivers;
using CSharpAutomation.Tests.Pages;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Reqnroll;

namespace CSharpAutomation.Tests.Steps
{
    [Binding]
    public class RegistrationSteps
    {
        private readonly RegistrationPage _registrationPage;
        private readonly IConfiguration _configuration;

        public RegistrationSteps(DriverFactory driverFactory, IConfiguration configuration)
        {
            _registrationPage = new RegistrationPage(driverFactory.Page);
            _configuration = configuration;
        }

        [Given(@"I navigate to the registration page")]
        public async Task GivenINavigateToTheRegistrationPage()
        {
            var baseUrl = _configuration["BaseUrl"];
            await _registrationPage.NavigateAsync($"{baseUrl}/register");
        }

        [When(@"I enter valid registration details")]
        public async Task WhenIEnterValidRegistrationDetails()
        {
            await _registrationPage.FillRegistrationFormAsync(
                "John Doe",
                "john@example.com",
                "SecurePass123!"
            );
        }

        [When(@"I submit the registration form")]
        public async Task WhenISubmitTheRegistrationForm()
        {
            await _registrationPage.SubmitFormAsync();
        }

        [Then(@"I should see a success message")]
        public async Task ThenIShouldSeeASuccessMessage()
        {
            var message = await _registrationPage.GetSuccessMessageAsync();
            Assert.That(message, Does.Contain("Registration successful"));
        }
    }
}
```

### 4. Build and Run

```powershell
dotnet build
dotnet test --filter "Category=smoke"
```

---

## 🏷️ Tags and Filtering

### Available Tag Levels

```gherkin
@Regression                              # Feature level tag
Feature: Login

@smoke @priority-high                    # Scenario level tags
Scenario: Quick login test
```

### Common Tags

| Tag | Purpose |
|-----|---------|
| `@sanity` | Basic sanity tests |
| `@smoke` | Quick smoke tests |
| `@Regression` | Full regression suite |
| `@LoginTest` | Login-related tests |
| `@priority-high` | Critical tests |
| `@wip` | Work in progress (skip) |

### Filter Examples

```powershell
# By category
dotnet test --filter "Category=sanity"

# By test name
dotnet test --filter "FullyQualifiedName~Login"

# Exclude tests
dotnet test --filter "Category!=wip"

# Complex filters
dotnet test --filter "(Category=sanity|Category=smoke)&Category!=wip"
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "BaseUrl": "https://hotelbooker.cert.sabre.com",
  "Browser": "chromium",
  "Headless": false,
  "SlowMo": 0,
  "Timeout": 30000,
  "ScreenshotsDir": "Screenshots"
}
```

### allureConfig.json

```json
{
  "allure": {
    "directory": "allure-results",
    "links": ["https://github.com/issues/{issue}"]
  }
}
```

---

## ✅ Best Practices

### 1. Page Object Design

```csharp
// ✅ Good: Use descriptive locator names
private string LoginButton => "#login-btn";

// ❌ Bad: Magic strings
await ClickAsync("#login-btn");
```

### 2. Wait Strategies

```csharp
// ✅ Good: Smart waits
await WaitForVisibleAsync("#element");

// ❌ Bad: Static waits
await WaitAsync(5000);
```

### 3. Assertions

```csharp
// ✅ Good: Specific assertions
await AssertTextContainsAsync("#msg", "Success");

// ❌ Bad: Generic assertions
Assert.That(true);
```

### 4. Step Definitions

```csharp
// ✅ Good: Reusable steps
[When(@"I enter username ""(.*)"" and password ""(.*)""")]

// ❌ Bad: Hardcoded values
[When(@"I enter username admin and password secret")]
```

### 5. Screenshots

```csharp
// ✅ Good: Take screenshots at key points
await TakeScreenshotAsync("AfterLogin");

// Automatic on failure (configured in hooks)
```

---

## 🔧 Troubleshooting

### Browser Not Installed

```powershell
# Error: Browser not found
# Solution:
dotnet exec bin/Debug/net10.0/Microsoft.Playwright.dll install
```

### Allure Report Shows "Loading..."

```powershell
# Issue: Opening HTML directly from file system
# Solution: Use allure server
C:\tools\allure\bin\allure.bat open "path\to\allure-report"
```

### Tests Timeout

```csharp
// Increase timeout in appsettings.json
"Timeout": 60000

// Or per test
await WaitForVisibleAsync("#element", 60000);
```

### Element Not Found

```csharp
// Debug with highlight
await HighlightAsync("#element");

// Check if in iframe
var frame = GetFrame("frameName");
await frame.ClickAsync("#element");
```

### Build Errors

```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

---

## 📞 Support

For issues or questions:
1. Check the troubleshooting section
2. Review Playwright documentation: https://playwright.dev/dotnet/
3. Review Reqnroll documentation: https://docs.reqnroll.net/

---

## 📄 License

This framework is for internal use. 

---

**Last Updated:** December 19, 2025
