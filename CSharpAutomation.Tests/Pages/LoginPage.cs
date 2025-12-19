using Microsoft.Playwright;

namespace CSharpAutomation.Tests.Pages
{
    public class LoginPage : BasePage
    {
        public LoginPage(IPage page) : base(page) { }

        // Locators based on the Sabre Hotel Booker app analyzed earlier
        private string UserNameField => "#ctl00_cphMainContent_txtUserName";
        private string PasswordField => "#ctl00_cphMainContent_txtPassword";
        private string LoginButton => "#ctl00_cphMainContent_btnLogin";

        public async Task LoginAsync(string username, string password)
        {
            await FillAsync(UserNameField, username);
            await FillAsync(PasswordField, password);
            await ClickAsync(LoginButton);
            await TakeScreenshotAsync("Login");
        }
    }
}
