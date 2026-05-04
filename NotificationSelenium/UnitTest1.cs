using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Xunit;


namespace NotificationSelenium
{
    public class UnitTest1
    {
        
        
            // Original URL (preserved)
            string url = "https://localhost:7159/api/Notification/ ";

            public void GetAllTest()
            {
                //IWebDriver driver = new ChromeDriver(DriverDirectory);
                //IWebDriver driver = new FirefoxDriver(DriverDirectory);
                IWebDriver driver = new EdgeDriver();
            }
    }
}
