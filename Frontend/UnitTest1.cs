using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Xunit;

public class TrashBinUiTests
{
    [Fact]
    public void Can_Create_TrashBin_Shows_Success_Message()
    {
        var driver = new ChromeDriver();
        driver.Navigate().GoToUrl("http://127.0.0.1:5500/index.html");

        driver.FindElement(By.CssSelector("input[placeholder='Navn']")).SendKeys("Test Bin");

        driver.FindElement(By.TagName("select")).SendKeys("Restaffald");

        driver.FindElement(By.CssSelector("input[placeholder='Lokation']")).SendKeys("Køge");

        driver.FindElement(By.CssSelector("button")).Click();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        var message = wait.Until(d =>
        {
            try
            {
                var el = d.FindElement(By.CssSelector(".text-success"));
                return el.Displayed ? el.Text : null;
            }
            catch
            {
                return null;
            }
        });

        Assert.Contains("oprettet", message.ToLower());

        driver.Quit();
    }
}