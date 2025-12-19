using Microsoft.Playwright;
using Allure.Net.Commons;
using CSharpAutomation.Tests.Utils;

namespace CSharpAutomation.Tests.Pages
{
    /// <summary>
    /// Base page class containing all reusable Playwright methods for page interactions.
    /// All page objects should inherit from this class.
    /// </summary>
    public abstract class BasePage
    {
        public readonly IPage Page;
        protected readonly int DefaultTimeout = 30000; // 30 seconds default timeout

        protected BasePage(IPage page)
        {
            Page = page;
        }

        #region Navigation Methods

        /// <summary>
        /// Navigate to a URL
        /// </summary>
        public async Task NavigateAsync(string url)
        {
            await Page.GotoAsync(url);
        }

        /// <summary>
        /// Navigate to URL and wait for specific load state
        /// </summary>
        public async Task NavigateAsync(string url, WaitUntilState waitUntil)
        {
            await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = waitUntil });
        }

        /// <summary>
        /// Navigate back in browser history
        /// </summary>
        public async Task GoBackAsync()
        {
            await Page.GoBackAsync();
        }

        /// <summary>
        /// Navigate forward in browser history
        /// </summary>
        public async Task GoForwardAsync()
        {
            await Page.GoForwardAsync();
        }

        /// <summary>
        /// Reload the current page
        /// </summary>
        public async Task ReloadAsync()
        {
            await Page.ReloadAsync();
        }

        /// <summary>
        /// Get the current page URL
        /// </summary>
        public string GetCurrentUrl()
        {
            return Page.Url;
        }

        /// <summary>
        /// Get the current page title
        /// </summary>
        public async Task<string> GetTitleAsync()
        {
            return await Page.TitleAsync();
        }

        #endregion

        #region Click Methods

        /// <summary>
        /// Click on an element
        /// </summary>
        public async Task ClickAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.ClickAsync(selector);
        }

        /// <summary>
        /// Click on an element with options
        /// </summary>
        public async Task ClickAsync(string selector, PageClickOptions options)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.ClickAsync(selector, options);
        }

        /// <summary>
        /// Double click on an element
        /// </summary>
        public async Task DoubleClickAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.DblClickAsync(selector);
        }

        /// <summary>
        /// Right click on an element
        /// </summary>
        public async Task RightClickAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.ClickAsync(selector, new PageClickOptions { Button = MouseButton.Right });
        }

        /// <summary>
        /// Force click on an element (bypasses actionability checks)
        /// </summary>
        public async Task ForceClickAsync(string selector)
        {
            await Page.ClickAsync(selector, new PageClickOptions { Force = true });
        }

        /// <summary>
        /// Click using JavaScript (useful when element is hidden or not clickable)
        /// </summary>
        public async Task JsClickAsync(string selector)
        {
            await Page.EvalOnSelectorAsync(selector, "element => element.click()");
        }

        #endregion

        #region Input Methods

        /// <summary>
        /// Fill text into an input field (clears existing text)
        /// </summary>
        public async Task FillAsync(string selector, string value)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.FillAsync(selector, value);
        }

        /// <summary>
        /// Type text into an input field (character by character)
        /// </summary>
        public async Task TypeAsync(string selector, string text, float delay = 0)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.Locator(selector).PressSequentiallyAsync(text, new LocatorPressSequentiallyOptions { Delay = delay });
        }

        /// <summary>
        /// Clear an input field
        /// </summary>
        public async Task ClearAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.FillAsync(selector, "");
        }

        /// <summary>
        /// Press a keyboard key
        /// </summary>
        public async Task PressKeyAsync(string key)
        {
            await Page.Keyboard.PressAsync(key);
        }

        /// <summary>
        /// Press a keyboard key on a specific element
        /// </summary>
        public async Task PressKeyAsync(string selector, string key)
        {
            await Page.Locator(selector).PressAsync(key);
        }

        /// <summary>
        /// Set value for input using JavaScript
        /// </summary>
        public async Task SetValueAsync(string selector, string value)
        {
            await Page.EvalOnSelectorAsync(selector, $"(element, value) => element.value = value", value);
        }

        #endregion

        #region Get Text/Attribute Methods

        /// <summary>
        /// Get text content of an element
        /// </summary>
        public async Task<string?> GetTextAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            return await Page.TextContentAsync(selector);
        }

        /// <summary>
        /// Get inner text of an element
        /// </summary>
        public async Task<string> GetInnerTextAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            return await Page.InnerTextAsync(selector);
        }

        /// <summary>
        /// Get inner HTML of an element
        /// </summary>
        public async Task<string> GetInnerHtmlAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            return await Page.InnerHTMLAsync(selector);
        }

        /// <summary>
        /// Get an attribute value of an element
        /// </summary>
        public async Task<string?> GetAttributeAsync(string selector, string attribute)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            return await Page.GetAttributeAsync(selector, attribute);
        }

        /// <summary>
        /// Get the input value of an element
        /// </summary>
        public async Task<string> GetInputValueAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            return await Page.InputValueAsync(selector);
        }

        /// <summary>
        /// Get all text contents from multiple elements
        /// </summary>
        public async Task<IReadOnlyList<string>> GetAllTextsAsync(string selector)
        {
            return await Page.Locator(selector).AllTextContentsAsync();
        }

        /// <summary>
        /// Get count of elements matching selector
        /// </summary>
        public async Task<int> GetElementCountAsync(string selector)
        {
            return await Page.Locator(selector).CountAsync();
        }

        #endregion

        #region Element State Methods

        /// <summary>
        /// Check if element is visible
        /// </summary>
        public async Task<bool> IsVisibleAsync(string selector)
        {
            return await Page.Locator(selector).IsVisibleAsync();
        }

        /// <summary>
        /// Check if element is hidden
        /// </summary>
        public async Task<bool> IsHiddenAsync(string selector)
        {
            return await Page.Locator(selector).IsHiddenAsync();
        }

        /// <summary>
        /// Check if element is enabled
        /// </summary>
        public async Task<bool> IsEnabledAsync(string selector)
        {
            return await Page.Locator(selector).IsEnabledAsync();
        }

        /// <summary>
        /// Check if element is disabled
        /// </summary>
        public async Task<bool> IsDisabledAsync(string selector)
        {
            return await Page.Locator(selector).IsDisabledAsync();
        }

        /// <summary>
        /// Check if element is checked (checkbox/radio)
        /// </summary>
        public async Task<bool> IsCheckedAsync(string selector)
        {
            return await Page.Locator(selector).IsCheckedAsync();
        }

        /// <summary>
        /// Check if element is editable
        /// </summary>
        public async Task<bool> IsEditableAsync(string selector)
        {
            return await Page.Locator(selector).IsEditableAsync();
        }

        /// <summary>
        /// Check if element exists in DOM
        /// </summary>
        public async Task<bool> ElementExistsAsync(string selector)
        {
            return await Page.Locator(selector).CountAsync() > 0;
        }

        #endregion

        #region Wait Methods

        /// <summary>
        /// Wait for an element to be visible
        /// </summary>
        public async Task WaitForVisibleAsync(string selector, int timeout = 30000)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });
        }

        /// <summary>
        /// Wait for an element to be hidden
        /// </summary>
        public async Task WaitForHiddenAsync(string selector, int timeout = 30000)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Hidden,
                Timeout = timeout
            });
        }

        /// <summary>
        /// Wait for an element to be attached to DOM
        /// </summary>
        public async Task WaitForAttachedAsync(string selector, int timeout = 30000)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Attached,
                Timeout = timeout
            });
        }

        /// <summary>
        /// Wait for an element to be detached from DOM
        /// </summary>
        public async Task WaitForDetachedAsync(string selector, int timeout = 30000)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Detached,
                Timeout = timeout
            });
        }

        /// <summary>
        /// Wait for page to reach a specific load state
        /// </summary>
        public async Task WaitForLoadStateAsync(LoadState state = LoadState.Load)
        {
            await Page.WaitForLoadStateAsync(state);
        }

        /// <summary>
        /// Wait for URL to contain specific text
        /// </summary>
        public async Task WaitForUrlAsync(string urlSubstring, int timeout = 30000)
        {
            await Page.WaitForURLAsync(url => url.Contains(urlSubstring), new PageWaitForURLOptions { Timeout = timeout });
        }

        /// <summary>
        /// Wait for a specific amount of time (use sparingly)
        /// </summary>
        public async Task WaitAsync(int milliseconds)
        {
            await Page.WaitForTimeoutAsync(milliseconds);
        }

        /// <summary>
        /// Wait for a network request to complete
        /// </summary>
        public async Task WaitForResponseAsync(string urlPattern)
        {
            await Page.WaitForResponseAsync(response => response.Url.Contains(urlPattern));
        }

        /// <summary>
        /// Wait for network to be idle
        /// </summary>
        public async Task WaitForNetworkIdleAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        #endregion

        #region Select/Dropdown Methods

        /// <summary>
        /// Select option from dropdown by value
        /// </summary>
        public async Task SelectByValueAsync(string selector, string value)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.SelectOptionAsync(selector, new SelectOptionValue { Value = value });
        }

        /// <summary>
        /// Select option from dropdown by label/text
        /// </summary>
        public async Task SelectByTextAsync(string selector, string text)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.SelectOptionAsync(selector, new SelectOptionValue { Label = text });
        }

        /// <summary>
        /// Select option from dropdown by index
        /// </summary>
        public async Task SelectByIndexAsync(string selector, int index)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.SelectOptionAsync(selector, new SelectOptionValue { Index = index });
        }

        /// <summary>
        /// Select multiple options from dropdown
        /// </summary>
        public async Task SelectMultipleAsync(string selector, string[] values)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.SelectOptionAsync(selector, values);
        }

        /// <summary>
        /// Get all options from a select dropdown
        /// </summary>
        public async Task<IReadOnlyList<string>> GetSelectOptionsAsync(string selector)
        {
            return await Page.Locator($"{selector} option").AllTextContentsAsync();
        }

        /// <summary>
        /// Get selected option text from dropdown
        /// </summary>
        public async Task<string> GetSelectedOptionTextAsync(string selector)
        {
            return await Page.Locator($"{selector} option:checked").TextContentAsync() ?? "";
        }

        #endregion

        #region Checkbox/Radio Methods

        /// <summary>
        /// Check a checkbox
        /// </summary>
        public async Task CheckAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.CheckAsync(selector);
        }

        /// <summary>
        /// Uncheck a checkbox
        /// </summary>
        public async Task UncheckAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.UncheckAsync(selector);
        }

        /// <summary>
        /// Set checkbox state
        /// </summary>
        public async Task SetCheckedAsync(string selector, bool isChecked)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.SetCheckedAsync(selector, isChecked);
        }

        #endregion

        #region Hover/Focus Methods

        /// <summary>
        /// Hover over an element
        /// </summary>
        public async Task HoverAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.HoverAsync(selector);
        }

        /// <summary>
        /// Focus on an element
        /// </summary>
        public async Task FocusAsync(string selector)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await Page.FocusAsync(selector);
        }

        /// <summary>
        /// Blur (unfocus) an element
        /// </summary>
        public async Task BlurAsync(string selector)
        {
            await Page.Locator(selector).BlurAsync();
        }

        #endregion

        #region Scroll Methods

        /// <summary>
        /// Scroll element into view
        /// </summary>
        public async Task ScrollIntoViewAsync(string selector)
        {
            await Page.Locator(selector).ScrollIntoViewIfNeededAsync();
        }

        /// <summary>
        /// Scroll to top of page
        /// </summary>
        public async Task ScrollToTopAsync()
        {
            await Page.EvaluateAsync("window.scrollTo(0, 0)");
        }

        /// <summary>
        /// Scroll to bottom of page
        /// </summary>
        public async Task ScrollToBottomAsync()
        {
            await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
        }

        /// <summary>
        /// Scroll by specific pixels
        /// </summary>
        public async Task ScrollByAsync(int x, int y)
        {
            await Page.EvaluateAsync($"window.scrollBy({x}, {y})");
        }

        /// <summary>
        /// Scroll to specific position
        /// </summary>
        public async Task ScrollToAsync(int x, int y)
        {
            await Page.EvaluateAsync($"window.scrollTo({x}, {y})");
        }

        #endregion

        #region Frame Methods

        /// <summary>
        /// Get a frame by name or URL
        /// </summary>
        public IFrame? GetFrame(string nameOrUrl)
        {
            return Page.Frame(nameOrUrl);
        }

        /// <summary>
        /// Get frame locator
        /// </summary>
        public IFrameLocator GetFrameLocator(string selector)
        {
            return Page.FrameLocator(selector);
        }

        /// <summary>
        /// Get all frames
        /// </summary>
        public IReadOnlyList<IFrame> GetAllFrames()
        {
            return Page.Frames;
        }

        #endregion

        #region Dialog/Alert Methods

        /// <summary>
        /// Accept dialog/alert
        /// </summary>
        public void HandleDialogAccept(string? promptText = null)
        {
            Page.Dialog += async (_, dialog) =>
            {
                if (promptText != null)
                    await dialog.AcceptAsync(promptText);
                else
                    await dialog.AcceptAsync();
            };
        }

        /// <summary>
        /// Dismiss dialog/alert
        /// </summary>
        public void HandleDialogDismiss()
        {
            Page.Dialog += async (_, dialog) => await dialog.DismissAsync();
        }

        #endregion

        #region File Upload Methods

        /// <summary>
        /// Upload a file
        /// </summary>
        public async Task UploadFileAsync(string selector, string filePath)
        {
            await Page.SetInputFilesAsync(selector, filePath);
        }

        /// <summary>
        /// Upload multiple files
        /// </summary>
        public async Task UploadFilesAsync(string selector, string[] filePaths)
        {
            await Page.SetInputFilesAsync(selector, filePaths);
        }

        /// <summary>
        /// Clear file input
        /// </summary>
        public async Task ClearFileInputAsync(string selector)
        {
            await Page.SetInputFilesAsync(selector, Array.Empty<string>());
        }

        #endregion

        #region Drag and Drop Methods

        /// <summary>
        /// Drag and drop from source to target
        /// </summary>
        public async Task DragAndDropAsync(string sourceSelector, string targetSelector)
        {
            await Page.DragAndDropAsync(sourceSelector, targetSelector);
        }

        #endregion

        #region JavaScript Execution Methods

        /// <summary>
        /// Execute JavaScript on page
        /// </summary>
        public async Task<T> ExecuteJsAsync<T>(string script)
        {
            return await Page.EvaluateAsync<T>(script);
        }

        /// <summary>
        /// Execute JavaScript on page (no return value)
        /// </summary>
        public async Task ExecuteJsAsync(string script)
        {
            await Page.EvaluateAsync(script);
        }

        /// <summary>
        /// Execute JavaScript on a specific element
        /// </summary>
        public async Task<T> ExecuteJsOnElementAsync<T>(string selector, string script)
        {
            return await Page.EvalOnSelectorAsync<T>(selector, script);
        }

        #endregion

        #region Screenshot Methods

        /// <summary>
        /// Take a screenshot and attach to Allure report
        /// </summary>
        public async Task TakeScreenshotAsync(string stepName)
        {
            try
            {
                // Get the project directory
                var currentDir = Directory.GetCurrentDirectory();
                var projectDir = currentDir;
                var dir = new DirectoryInfo(currentDir);
                while (dir != null)
                {
                    if (dir.GetFiles("*.csproj").Length > 0)
                    {
                        projectDir = dir.FullName;
                        break;
                    }
                    dir = dir.Parent;
                }

                // Create timestamped folder structure: reports/19DEC/1603/Screenshots/
                var now = DateTime.Now;
                var dateFolder = now.ToString("ddMMM").ToUpper();
                var timeFolder = now.ToString("HHmm");
                var screenshotDir = Path.Combine(projectDir, "reports", dateFolder, timeFolder, "Screenshots");
                
                if (!Directory.Exists(screenshotDir))
                {
                    Directory.CreateDirectory(screenshotDir);
                }

                var screenshotPath = Path.Combine(screenshotDir, $"{stepName}_{now:HHmmss}.png");
                
                var screenshotBytes = await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });
                
                // Attach screenshot to Allure report
                try
                {
                    AllureApi.AddAttachment(stepName, "image/png", screenshotBytes);
                }
                catch
                {
                    // Allure context may not be active, skip attachment
                }
                
                Logger.Success($"Screenshot saved: {screenshotPath}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to take screenshot: {ex.Message}");
            }
        }

        /// <summary>
        /// Take a screenshot of a specific element
        /// </summary>
        public async Task<byte[]> TakeElementScreenshotAsync(string selector, string? path = null)
        {
            var options = new LocatorScreenshotOptions();
            if (path != null)
            {
                options.Path = path;
            }
            return await Page.Locator(selector).ScreenshotAsync(options);
        }

        /// <summary>
        /// Take a full page screenshot
        /// </summary>
        public async Task<byte[]> TakeFullPageScreenshotAsync(string? path = null)
        {
            var options = new PageScreenshotOptions { FullPage = true };
            if (path != null)
            {
                options.Path = path;
            }
            return await Page.ScreenshotAsync(options);
        }

        #endregion

        #region Locator Methods

        /// <summary>
        /// Get a locator for an element
        /// </summary>
        public ILocator Locator(string selector)
        {
            return Page.Locator(selector);
        }

        /// <summary>
        /// Get element by role
        /// </summary>
        public ILocator GetByRole(AriaRole role, PageGetByRoleOptions? options = null)
        {
            return Page.GetByRole(role, options);
        }

        /// <summary>
        /// Get element by text
        /// </summary>
        public ILocator GetByText(string text, PageGetByTextOptions? options = null)
        {
            return Page.GetByText(text, options);
        }

        /// <summary>
        /// Get element by label
        /// </summary>
        public ILocator GetByLabel(string text, PageGetByLabelOptions? options = null)
        {
            return Page.GetByLabel(text, options);
        }

        /// <summary>
        /// Get element by placeholder
        /// </summary>
        public ILocator GetByPlaceholder(string text, PageGetByPlaceholderOptions? options = null)
        {
            return Page.GetByPlaceholder(text, options);
        }

        /// <summary>
        /// Get element by alt text
        /// </summary>
        public ILocator GetByAltText(string text, PageGetByAltTextOptions? options = null)
        {
            return Page.GetByAltText(text, options);
        }

        /// <summary>
        /// Get element by title
        /// </summary>
        public ILocator GetByTitle(string text, PageGetByTitleOptions? options = null)
        {
            return Page.GetByTitle(text, options);
        }

        /// <summary>
        /// Get element by test id
        /// </summary>
        public ILocator GetByTestId(string testId)
        {
            return Page.GetByTestId(testId);
        }

        #endregion

        #region Browser Context Methods

        /// <summary>
        /// Get all cookies
        /// </summary>
        public async Task<IReadOnlyList<BrowserContextCookiesResult>> GetCookiesAsync()
        {
            return await Page.Context.CookiesAsync();
        }

        /// <summary>
        /// Add cookies
        /// </summary>
        public async Task AddCookiesAsync(IEnumerable<Cookie> cookies)
        {
            await Page.Context.AddCookiesAsync(cookies);
        }

        /// <summary>
        /// Clear cookies
        /// </summary>
        public async Task ClearCookiesAsync()
        {
            await Page.Context.ClearCookiesAsync();
        }

        /// <summary>
        /// Get local storage item
        /// </summary>
        public async Task<string?> GetLocalStorageItemAsync(string key)
        {
            return await Page.EvaluateAsync<string?>($"localStorage.getItem('{key}')");
        }

        /// <summary>
        /// Set local storage item
        /// </summary>
        public async Task SetLocalStorageItemAsync(string key, string value)
        {
            await Page.EvaluateAsync($"localStorage.setItem('{key}', '{value}')");
        }

        /// <summary>
        /// Clear local storage
        /// </summary>
        public async Task ClearLocalStorageAsync()
        {
            await Page.EvaluateAsync("localStorage.clear()");
        }

        /// <summary>
        /// Get session storage item
        /// </summary>
        public async Task<string?> GetSessionStorageItemAsync(string key)
        {
            return await Page.EvaluateAsync<string?>($"sessionStorage.getItem('{key}')");
        }

        /// <summary>
        /// Set session storage item
        /// </summary>
        public async Task SetSessionStorageItemAsync(string key, string value)
        {
            await Page.EvaluateAsync($"sessionStorage.setItem('{key}', '{value}')");
        }

        /// <summary>
        /// Clear session storage
        /// </summary>
        public async Task ClearSessionStorageAsync()
        {
            await Page.EvaluateAsync("sessionStorage.clear()");
        }

        #endregion

        #region Keyboard Methods

        /// <summary>
        /// Press Enter key
        /// </summary>
        public async Task PressEnterAsync()
        {
            await Page.Keyboard.PressAsync("Enter");
        }

        /// <summary>
        /// Press Tab key
        /// </summary>
        public async Task PressTabAsync()
        {
            await Page.Keyboard.PressAsync("Tab");
        }

        /// <summary>
        /// Press Escape key
        /// </summary>
        public async Task PressEscapeAsync()
        {
            await Page.Keyboard.PressAsync("Escape");
        }

        /// <summary>
        /// Press keyboard shortcut (e.g., "Control+A", "Control+C")
        /// </summary>
        public async Task PressShortcutAsync(string shortcut)
        {
            await Page.Keyboard.PressAsync(shortcut);
        }

        /// <summary>
        /// Type text using keyboard
        /// </summary>
        public async Task KeyboardTypeAsync(string text)
        {
            await Page.Keyboard.TypeAsync(text);
        }

        #endregion

        #region Mouse Methods

        /// <summary>
        /// Move mouse to specific coordinates
        /// </summary>
        public async Task MouseMoveAsync(float x, float y)
        {
            await Page.Mouse.MoveAsync(x, y);
        }

        /// <summary>
        /// Click at specific coordinates
        /// </summary>
        public async Task MouseClickAsync(float x, float y)
        {
            await Page.Mouse.ClickAsync(x, y);
        }

        /// <summary>
        /// Double click at specific coordinates
        /// </summary>
        public async Task MouseDoubleClickAsync(float x, float y)
        {
            await Page.Mouse.DblClickAsync(x, y);
        }

        /// <summary>
        /// Mouse wheel scroll
        /// </summary>
        public async Task MouseWheelAsync(float deltaX, float deltaY)
        {
            await Page.Mouse.WheelAsync(deltaX, deltaY);
        }

        #endregion

        #region Highlight Methods (for debugging)

        /// <summary>
        /// Highlight an element (for debugging)
        /// </summary>
        public async Task HighlightAsync(string selector)
        {
            await Page.Locator(selector).HighlightAsync();
        }

        #endregion

        #region Window/Tab Management Methods

        /// <summary>
        /// Get all pages/tabs in the browser context
        /// </summary>
        public IReadOnlyList<IPage> GetAllPages()
        {
            return Page.Context.Pages;
        }

        /// <summary>
        /// Get the count of open pages/tabs
        /// </summary>
        public int GetPageCount()
        {
            return Page.Context.Pages.Count;
        }

        /// <summary>
        /// Wait for a new page/tab to open and return it
        /// </summary>
        public async Task<IPage> WaitForNewPageAsync(Func<Task> action)
        {
            var newPageTask = Page.Context.WaitForPageAsync();
            await action();
            return await newPageTask;
        }

        /// <summary>
        /// Switch to a new page/tab by index
        /// </summary>
        public IPage SwitchToPageByIndex(int index)
        {
            var pages = Page.Context.Pages;
            if (index < 0 || index >= pages.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Page index {index} is out of range. Total pages: {pages.Count}");
            }
            return pages[index];
        }

        /// <summary>
        /// Switch to a page/tab by URL (partial match)
        /// </summary>
        public IPage? SwitchToPageByUrl(string urlSubstring)
        {
            return Page.Context.Pages.FirstOrDefault(p => p.Url.Contains(urlSubstring));
        }

        /// <summary>
        /// Switch to a page/tab by title (partial match)
        /// </summary>
        public async Task<IPage?> SwitchToPageByTitleAsync(string titleSubstring)
        {
            foreach (var page in Page.Context.Pages)
            {
                var title = await page.TitleAsync();
                if (title.Contains(titleSubstring))
                {
                    return page;
                }
            }
            return null;
        }

        /// <summary>
        /// Close the current page and switch to another page
        /// </summary>
        public async Task<IPage?> CloseCurrentPageAndSwitchAsync()
        {
            var pages = Page.Context.Pages;
            var currentIndex = pages.ToList().IndexOf(Page);
            
            await Page.CloseAsync();
            
            // Get remaining pages
            var remainingPages = Page.Context.Pages;
            if (remainingPages.Count > 0)
            {
                // Switch to previous page or first available
                var newIndex = Math.Min(currentIndex, remainingPages.Count - 1);
                return remainingPages[newIndex >= 0 ? newIndex : 0];
            }
            return null;
        }

        /// <summary>
        /// Close page by index
        /// </summary>
        public async Task ClosePageByIndexAsync(int index)
        {
            var pages = Page.Context.Pages;
            if (index >= 0 && index < pages.Count)
            {
                await pages[index].CloseAsync();
            }
        }

        /// <summary>
        /// Close all pages except current
        /// </summary>
        public async Task CloseOtherPagesAsync()
        {
            var pages = Page.Context.Pages.ToList();
            foreach (var page in pages)
            {
                if (page != Page)
                {
                    await page.CloseAsync();
                }
            }
        }

        /// <summary>
        /// Open a new page/tab
        /// </summary>
        public async Task<IPage> OpenNewPageAsync()
        {
            return await Page.Context.NewPageAsync();
        }

        /// <summary>
        /// Open a new page/tab and navigate to URL
        /// </summary>
        public async Task<IPage> OpenNewPageAsync(string url)
        {
            var newPage = await Page.Context.NewPageAsync();
            await newPage.GotoAsync(url);
            return newPage;
        }

        /// <summary>
        /// Bring page to front (focus)
        /// </summary>
        public async Task BringToFrontAsync()
        {
            await Page.BringToFrontAsync();
        }

        #endregion

        #region Alert/Dialog Handling Methods

        /// <summary>
        /// Setup handler to accept all dialogs automatically
        /// </summary>
        public void SetupAutoAcceptDialogs()
        {
            Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        }

        /// <summary>
        /// Setup handler to dismiss all dialogs automatically
        /// </summary>
        public void SetupAutoDismissDialogs()
        {
            Page.Dialog += async (_, dialog) => await dialog.DismissAsync();
        }

        /// <summary>
        /// Handle the next dialog with accept
        /// </summary>
        public async Task<string> AcceptNextDialogAsync(Func<Task> triggerAction)
        {
            var dialogMessage = "";
            void handler(object? sender, IDialog dialog)
            {
                dialogMessage = dialog.Message;
                dialog.AcceptAsync().GetAwaiter().GetResult();
                Page.Dialog -= handler;
            }
            Page.Dialog += handler;
            await triggerAction();
            await WaitAsync(500); // Small wait for dialog to be processed
            return dialogMessage;
        }

        /// <summary>
        /// Handle the next dialog with dismiss
        /// </summary>
        public async Task<string> DismissNextDialogAsync(Func<Task> triggerAction)
        {
            var dialogMessage = "";
            void handler(object? sender, IDialog dialog)
            {
                dialogMessage = dialog.Message;
                dialog.DismissAsync().GetAwaiter().GetResult();
                Page.Dialog -= handler;
            }
            Page.Dialog += handler;
            await triggerAction();
            await WaitAsync(500);
            return dialogMessage;
        }

        /// <summary>
        /// Handle prompt dialog with text input
        /// </summary>
        public async Task<string> HandlePromptDialogAsync(Func<Task> triggerAction, string promptText)
        {
            var dialogMessage = "";
            void handler(object? sender, IDialog dialog)
            {
                dialogMessage = dialog.Message;
                dialog.AcceptAsync(promptText).GetAwaiter().GetResult();
                Page.Dialog -= handler;
            }
            Page.Dialog += handler;
            await triggerAction();
            await WaitAsync(500);
            return dialogMessage;
        }

        /// <summary>
        /// Get dialog type (alert, confirm, prompt, beforeunload)
        /// </summary>
        public async Task<(string Type, string Message)> GetNextDialogInfoAsync(Func<Task> triggerAction)
        {
            var dialogType = "";
            var dialogMessage = "";
            void handler(object? sender, IDialog dialog)
            {
                dialogType = dialog.Type;
                dialogMessage = dialog.Message;
                dialog.AcceptAsync().GetAwaiter().GetResult();
                Page.Dialog -= handler;
            }
            Page.Dialog += handler;
            await triggerAction();
            await WaitAsync(500);
            return (dialogType, dialogMessage);
        }

        #endregion

        #region Assertion Methods

        /// <summary>
        /// Assert that element contains expected text
        /// </summary>
        public async Task AssertTextContainsAsync(string selector, string expectedText)
        {
            var actualText = await GetTextAsync(selector);
            if (actualText == null || !actualText.Contains(expectedText))
            {
                throw new AssertionException($"Expected element '{selector}' to contain text '{expectedText}', but got '{actualText}'");
            }
        }

        /// <summary>
        /// Assert that element text equals expected text
        /// </summary>
        public async Task AssertTextEqualsAsync(string selector, string expectedText)
        {
            var actualText = await GetTextAsync(selector);
            if (actualText != expectedText)
            {
                throw new AssertionException($"Expected element '{selector}' text to equal '{expectedText}', but got '{actualText}'");
            }
        }

        /// <summary>
        /// Assert that element is visible
        /// </summary>
        public async Task AssertVisibleAsync(string selector)
        {
            var isVisible = await IsVisibleAsync(selector);
            if (!isVisible)
            {
                throw new AssertionException($"Expected element '{selector}' to be visible, but it was not");
            }
        }

        /// <summary>
        /// Assert that element is hidden
        /// </summary>
        public async Task AssertHiddenAsync(string selector)
        {
            var isHidden = await IsHiddenAsync(selector);
            if (!isHidden)
            {
                throw new AssertionException($"Expected element '{selector}' to be hidden, but it was visible");
            }
        }

        /// <summary>
        /// Assert that element is enabled
        /// </summary>
        public async Task AssertEnabledAsync(string selector)
        {
            var isEnabled = await IsEnabledAsync(selector);
            if (!isEnabled)
            {
                throw new AssertionException($"Expected element '{selector}' to be enabled, but it was disabled");
            }
        }

        /// <summary>
        /// Assert that element is disabled
        /// </summary>
        public async Task AssertDisabledAsync(string selector)
        {
            var isDisabled = await IsDisabledAsync(selector);
            if (!isDisabled)
            {
                throw new AssertionException($"Expected element '{selector}' to be disabled, but it was enabled");
            }
        }

        /// <summary>
        /// Assert that checkbox is checked
        /// </summary>
        public async Task AssertCheckedAsync(string selector)
        {
            var isChecked = await IsCheckedAsync(selector);
            if (!isChecked)
            {
                throw new AssertionException($"Expected element '{selector}' to be checked, but it was not");
            }
        }

        /// <summary>
        /// Assert that checkbox is not checked
        /// </summary>
        public async Task AssertNotCheckedAsync(string selector)
        {
            var isChecked = await IsCheckedAsync(selector);
            if (isChecked)
            {
                throw new AssertionException($"Expected element '{selector}' to be unchecked, but it was checked");
            }
        }

        /// <summary>
        /// Assert that element exists in DOM
        /// </summary>
        public async Task AssertExistsAsync(string selector)
        {
            var exists = await ElementExistsAsync(selector);
            if (!exists)
            {
                throw new AssertionException($"Expected element '{selector}' to exist, but it was not found");
            }
        }

        /// <summary>
        /// Assert that element does not exist in DOM
        /// </summary>
        public async Task AssertNotExistsAsync(string selector)
        {
            var exists = await ElementExistsAsync(selector);
            if (exists)
            {
                throw new AssertionException($"Expected element '{selector}' to not exist, but it was found");
            }
        }

        /// <summary>
        /// Assert element has specific attribute value
        /// </summary>
        public async Task AssertAttributeEqualsAsync(string selector, string attribute, string expectedValue)
        {
            var actualValue = await GetAttributeAsync(selector, attribute);
            if (actualValue != expectedValue)
            {
                throw new AssertionException($"Expected element '{selector}' attribute '{attribute}' to equal '{expectedValue}', but got '{actualValue}'");
            }
        }

        /// <summary>
        /// Assert element attribute contains value
        /// </summary>
        public async Task AssertAttributeContainsAsync(string selector, string attribute, string expectedValue)
        {
            var actualValue = await GetAttributeAsync(selector, attribute);
            if (actualValue == null || !actualValue.Contains(expectedValue))
            {
                throw new AssertionException($"Expected element '{selector}' attribute '{attribute}' to contain '{expectedValue}', but got '{actualValue}'");
            }
        }

        /// <summary>
        /// Assert element has specific CSS class
        /// </summary>
        public async Task AssertHasClassAsync(string selector, string className)
        {
            var classAttr = await GetAttributeAsync(selector, "class");
            if (classAttr == null || !classAttr.Split(' ').Contains(className))
            {
                throw new AssertionException($"Expected element '{selector}' to have class '{className}', but got '{classAttr}'");
            }
        }

        /// <summary>
        /// Assert element count equals expected
        /// </summary>
        public async Task AssertElementCountAsync(string selector, int expectedCount)
        {
            var actualCount = await GetElementCountAsync(selector);
            if (actualCount != expectedCount)
            {
                throw new AssertionException($"Expected {expectedCount} elements matching '{selector}', but found {actualCount}");
            }
        }

        /// <summary>
        /// Assert element count is greater than
        /// </summary>
        public async Task AssertElementCountGreaterThanAsync(string selector, int count)
        {
            var actualCount = await GetElementCountAsync(selector);
            if (actualCount <= count)
            {
                throw new AssertionException($"Expected more than {count} elements matching '{selector}', but found {actualCount}");
            }
        }

        /// <summary>
        /// Assert URL contains expected text
        /// </summary>
        public void AssertUrlContains(string expectedText)
        {
            var currentUrl = GetCurrentUrl();
            if (!currentUrl.Contains(expectedText))
            {
                throw new AssertionException($"Expected URL to contain '{expectedText}', but got '{currentUrl}'");
            }
        }

        /// <summary>
        /// Assert URL equals expected
        /// </summary>
        public void AssertUrlEquals(string expectedUrl)
        {
            var currentUrl = GetCurrentUrl();
            if (currentUrl != expectedUrl)
            {
                throw new AssertionException($"Expected URL to equal '{expectedUrl}', but got '{currentUrl}'");
            }
        }

        /// <summary>
        /// Assert page title contains expected text
        /// </summary>
        public async Task AssertTitleContainsAsync(string expectedText)
        {
            var title = await GetTitleAsync();
            if (!title.Contains(expectedText))
            {
                throw new AssertionException($"Expected title to contain '{expectedText}', but got '{title}'");
            }
        }

        /// <summary>
        /// Assert page title equals expected text
        /// </summary>
        public async Task AssertTitleEqualsAsync(string expectedText)
        {
            var title = await GetTitleAsync();
            if (title != expectedText)
            {
                throw new AssertionException($"Expected title to equal '{expectedText}', but got '{title}'");
            }
        }

        /// <summary>
        /// Assert input value equals expected
        /// </summary>
        public async Task AssertInputValueAsync(string selector, string expectedValue)
        {
            var actualValue = await GetInputValueAsync(selector);
            if (actualValue != expectedValue)
            {
                throw new AssertionException($"Expected input '{selector}' value to equal '{expectedValue}', but got '{actualValue}'");
            }
        }

        /// <summary>
        /// Assert with custom condition
        /// </summary>
        public void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Assert using Playwright's built-in expect (recommended)
        /// </summary>
        public ILocatorAssertions Expect(string selector)
        {
            return Assertions.Expect(Page.Locator(selector));
        }

        /// <summary>
        /// Assert using Playwright's built-in expect for page
        /// </summary>
        public IPageAssertions ExpectPage()
        {
            return Assertions.Expect(Page);
        }

        #endregion

        #region Soft Assertions (collect failures without stopping)

        private List<string> _softAssertionErrors = new List<string>();

        /// <summary>
        /// Soft assert - collect errors without stopping test
        /// </summary>
        public async Task SoftAssertTextContainsAsync(string selector, string expectedText)
        {
            try
            {
                await AssertTextContainsAsync(selector, expectedText);
            }
            catch (AssertionException ex)
            {
                _softAssertionErrors.Add(ex.Message);
            }
        }

        /// <summary>
        /// Soft assert - element is visible
        /// </summary>
        public async Task SoftAssertVisibleAsync(string selector)
        {
            try
            {
                await AssertVisibleAsync(selector);
            }
            catch (AssertionException ex)
            {
                _softAssertionErrors.Add(ex.Message);
            }
        }

        /// <summary>
        /// Get all soft assertion errors
        /// </summary>
        public IReadOnlyList<string> GetSoftAssertionErrors()
        {
            return _softAssertionErrors.AsReadOnly();
        }

        /// <summary>
        /// Assert all soft assertions passed (call at end of test)
        /// </summary>
        public void AssertAllSoftAssertions()
        {
            if (_softAssertionErrors.Count > 0)
            {
                var allErrors = string.Join("\n", _softAssertionErrors);
                _softAssertionErrors.Clear();
                throw new AssertionException($"Soft assertion failures:\n{allErrors}");
            }
        }

        /// <summary>
        /// Clear soft assertion errors
        /// </summary>
        public void ClearSoftAssertions()
        {
            _softAssertionErrors.Clear();
        }

        #endregion
    }

    /// <summary>
    /// Custom assertion exception
    /// </summary>
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}
