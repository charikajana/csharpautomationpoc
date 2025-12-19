using CSharpAutomation.Tests.Drivers;
using CSharpAutomation.Tests.Pages;
using Microsoft.Extensions.Configuration;
using Reqnroll;
using NUnit.Framework;

namespace CSharpAutomation.Tests.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly LoginPage _loginPage;
        private readonly IConfiguration _configuration;

        public LoginSteps(DriverFactory driverFactory, IConfiguration configuration)
        {
            _loginPage = new LoginPage(driverFactory.Page);
            _configuration = configuration;
        }

        [Given(@"I navigate to the login page")]
        public async Task GivenINavigateToTheLoginPage()
        {
            await _loginPage.NavigateAsync(_configuration["BaseUrl"]!);
        }

        [When(@"I enter username ""(.*)"" and password ""(.*)""")]
        public async Task WhenIEnterUsernameAndPassword(string user, string pass)
        {
            await _loginPage.LoginAsync(user, pass);
        }

        [When(@"I click the login button")]
        public async Task WhenIClickTheLoginButton()
        {
            // Handled in LoginAsync above, but separated here to match the feature
        }

        [Then(@"I should be logged in successfully")]
        public async Task ThenIShouldBeLoggedInSuccessfully()
        {
            Assert.That(await _loginPage.Page.TitleAsync(), Does.Contain("Hotel Booker"));
        }
    }
}
