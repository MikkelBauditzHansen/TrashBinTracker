using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace SeleniumUITDD
{
    public class NotificationSettingsTests : IDisposable
    {
        private readonly IWebDriver _driver;

        public NotificationSettingsTests()
        {
            _driver = new ChromeDriver();
        }

        [Fact]
        public void AdminCanOpenNotificationSettings()
        {
            _driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;

            js.ExecuteScript("localStorage.setItem('token', 'fake-token');");
            js.ExecuteScript("localStorage.setItem('username', 'admin');");
            js.ExecuteScript("localStorage.setItem('role', 'Admin');");

            _driver.Navigate().GoToUrl("http://127.0.0.1:5500/notification-settings.html");

            string title = _driver.FindElement(By.Id("pageTitle")).Text;

            Assert.Contains("Konfigurer notifikationer", title);
        }

        [Fact]
        public void NormalUserCannotOpenNotificationSettings()
        {
            _driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;

            js.ExecuteScript("localStorage.setItem('token', 'fake-token');");
            js.ExecuteScript("localStorage.setItem('username', 'user');");
            js.ExecuteScript("localStorage.setItem('role', 'User');");

            _driver.Navigate().GoToUrl("http://127.0.0.1:5500/notification-settings.html");

            string message = _driver.FindElement(By.Id("accessDenied")).Text;

            Assert.Contains("Du har ikke adgang", message);
        }

        [Fact]
        public void AdminCanToggleTelegramForUser()
        {
            _driver.Navigate().GoToUrl("http://127.0.0.1:5500/login.html");

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;

            js.ExecuteScript("localStorage.setItem('token', 'fake-token');");
            js.ExecuteScript("localStorage.setItem('username', 'admin');");
            js.ExecuteScript("localStorage.setItem('role', 'Admin');");

            _driver.Navigate().GoToUrl("http://127.0.0.1:5500/notification-settings.html");

            IWebElement telegramCheckbox = _driver.FindElement(By.Id("telegram-admin"));
            bool before = telegramCheckbox.Selected;

            telegramCheckbox.Click();

            bool after = telegramCheckbox.Selected;

            Assert.NotEqual(before, after);
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}