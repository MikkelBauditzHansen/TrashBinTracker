using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace TrashBinTracker.Tests
{
    public class TrashBinUiTests
    {
        [Fact]
        public void Can_Create_TrashBin_Shows_Success_Message()
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                driver.Navigate().GoToUrl("http://127.0.0.1:5500/index.html");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                wait.Until(d => d.FindElement(By.CssSelector("input[placeholder='Navn']")))
                    .SendKeys("Test Bin");

                driver.FindElement(By.TagName("select")).SendKeys("Restaffald");

                driver.FindElement(By.CssSelector("input[placeholder='Lokation']"))
                    .SendKeys("Køge");

                driver.FindElement(By.CssSelector("button.btn-primary")).Click();

                string pageText = wait.Until(d =>
                {
                    string bodyText = d.FindElement(By.TagName("body")).Text;

                    if (bodyText.ToLower().Contains("oprettet") ||
                        bodyText.ToLower().Contains("fejl") ||
                        bodyText.ToLower().Contains("kunne ikke"))
                    {
                        return bodyText;
                    }

                    return null;
                });

                Assert.Contains("oprettet", pageText.ToLower());
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}