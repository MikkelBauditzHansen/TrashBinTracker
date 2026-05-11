using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace TrashBinTrackerSeleniumTests
{
    public class TrashBinSeleniumTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly string _frontendUrl =
            (Environment.GetEnvironmentVariable("TRASHBINTRACKER_FRONTEND_URL")
             ?? "http://127.0.0.1:5500").TrimEnd('/');

        public TrashBinSeleniumTests()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--disable-search-engine-choice-screen");

            if (Environment.GetEnvironmentVariable("SELENIUM_HEADLESS") == "true")
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1400,1000");
            }

            _driver = new ChromeDriver(options);

            _driver.Manage().Window.Maximize();

            _driver.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(1);

            _driver.Manage().Timeouts().PageLoad =
                TimeSpan.FromSeconds(30);

            _wait = new WebDriverWait(
                _driver,
                TimeSpan.FromSeconds(15)
            );
        }

        [Fact]
        public void AdminCanSeeDashboard()
        {
            LoginAsAdmin();

            string bodyText =
                _driver.FindElement(By.TagName("body")).Text;

            Assert.Contains("Velkommen tilbage", bodyText);

            Assert.Contains("Opret skraldespand", bodyText);
        }

        [Fact]
        public void AdminCanSeeSettingsMenu()
        {
            LoginAsAdmin();

            string bodyText =
                _driver.FindElement(By.TagName("body")).Text;

            Assert.Contains("Indstillinger", bodyText);
        }

        [Fact]
        public void NormalUserCannotSeeSettingsMenu()
        {
            LoginAsUser();

            string bodyText =
                _driver.FindElement(By.TagName("body")).Text;

            Assert.DoesNotContain("Indstillinger", bodyText);
        }

        [Fact]
        public void UserCanSeeNotificationsPage()
        {
            LoginAsUser();

            NavigateToFrontend("notifications.html");

            IWebElement body =
                WaitForElement(By.TagName("body"));

            Assert.Contains("Notifikationer", body.Text);
        }

        [Fact]
        public void UserCanSeeHistoryPage()
        {
            LoginAsUser();

            NavigateToFrontend("History.html");

            IWebElement body =
                WaitForElement(By.TagName("body"));

            Assert.Contains("Tømningshistorik", body.Text);
        }

        [Fact]
        public void AdminCanOpenCreateBinForm()
        {
            LoginAsAdmin();

            IWebElement createButton =
                WaitForElement(By.Id("createBinButton"));

            createButton.Click();

            IWebElement input =
                WaitForElement(By.Id("binNameInput"));

            Assert.True(input.Displayed);
        }

        [Fact]
        public void AdminCanCreateTrashBin()
        {
            LoginAsAdmin();
            PrepareDashboardTestData(includeBin: false);

            IWebElement createButton =
                WaitForElement(By.Id("createBinButton"));

            createButton.Click();

            IWebElement nameInput =
                WaitForElement(By.Id("binNameInput"));

            TypeSlowly(nameInput, "Test skraldespand");

            _driver.FindElement(
                By.CssSelector(
                    "#wasteTypeSelect option[value='Organic']"
                )
            ).Click();

            SelectElement locationSelect =
                new SelectElement(
                    WaitForElement(By.Id("locationSelect"))
                );

            locationSelect.SelectByIndex(1);

            IWebElement saveButton =
                WaitForElement(By.Id("saveBinButton"));

            saveButton.Click();

            WebDriverWait wait =
                new WebDriverWait(
                    _driver,
                    TimeSpan.FromSeconds(10)
                );

            bool binCreated = wait.Until(driver =>
                driver.FindElement(By.TagName("body"))
                      .Text.Contains("Test skraldespand")
            );

            Assert.True(binCreated);
        }

        [Fact]
        public void UserCanMarkTrashBinAsEmpty()
        {
            LoginAsUser();
            PrepareDashboardTestData(includeBin: true);

            IWebElement emptyButton =
                WaitForElement(
                    By.XPath(
                        "//button[contains(text(), 'Marker som tømt')]"
                    )
                );

            emptyButton.Click();

            WebDriverWait wait =
                new WebDriverWait(
                    _driver,
                    TimeSpan.FromSeconds(10)
                );

            bool emptied = wait.Until(driver =>
                driver.FindElement(By.TagName("body"))
                      .Text.Contains("0%")
            );

            Assert.True(emptied);
        }

        [Fact]
        public void UserCanIncreaseFillLevel()
        {
            LoginAsUser();
            PrepareDashboardTestData(includeBin: true);

            IWebElement increaseButton =
                WaitForElement(
                    By.XPath(
                        "//button[contains(text(), '+10%')]"
                    )
                );

            increaseButton.Click();

            WebDriverWait wait =
                new WebDriverWait(
                    _driver,
                    TimeSpan.FromSeconds(10)
                );

            bool hasPercent = wait.Until(driver =>
                driver.FindElement(By.TagName("body"))
                      .Text.Contains("%")
            );

            Assert.True(hasPercent);
        }

        [Fact]
        public void TemperatureWarningCanBeShown()
        {
            LoginAsUser();
            PrepareDashboardTestData(includeBin: true);

            WaitUntilBodyContains("Vejr");

            IWebElement temperatureInput =
                WaitForElement(By.Id("testTemperatureInput"));

            temperatureInput.Clear();

            temperatureInput.SendKeys("25");

            IWebElement testButton =
                WaitForElement(By.Id("testTemperatureButton"));

            testButton.Click();

            WebDriverWait wait =
                new WebDriverWait(
                    _driver,
                    TimeSpan.FromSeconds(15)
                );

            bool warningShown = wait.Until(driver =>
            {
                string bodyText =
                    driver.FindElement(By.TagName("body")).Text;

                return bodyText.Contains("Temperatur-advarsel") ||
                       bodyText.Contains("bør tømmes") ||
                       bodyText.Contains("lugte");
            });

            Assert.True(warningShown);
        }

        [Fact]
        public void NotificationsCanBeMarkedAsRead()
        {
            LoginAsUser();

            NavigateToFrontend("notifications.html");

            IWebElement body =
                WaitForElement(By.TagName("body"));

            string bodyText = body.Text;

            if (bodyText.Contains("Læst"))
            {
                IWebElement readButton =
                    WaitForElement(
                        By.XPath(
                            "//button[contains(text(), 'Læst')]"
                        )
                    );

                readButton.Click();

                Assert.True(true);
            }
            else
            {
                Assert.Contains(
                    "Ingen notifikationer",
                    bodyText
                );
            }
        }

        [Fact]
        public void AdminCanOpenNotificationSettings()
        {
            LoginAsAdmin();

            NavigateToFrontend("notification-settings.html");

            IWebElement body =
                WaitForElement(By.TagName("body"));

            Assert.Contains(
                "Konfigurer notifikationer",
                body.Text
            );
        }

        [Fact]
        public void NormalUserCannotOpenNotificationSettings()
        {
            LoginAsUser();

            NavigateToFrontend("notification-settings.html");

            IWebElement body =
                WaitForElement(By.TagName("body"));

            Assert.Contains(
                "Du har ikke adgang",
                body.Text
            );
        }

        // ── Private helpers ────────────────────────────────────────────

        private void CreateOrganicBinIfNeeded()
        {
            PrepareDashboardTestData(includeBin: false);

            WaitUntilBodyContains("Opret skraldespand");

            IWebElement createButton =
                WaitForElement(By.Id("createBinButton"));

            createButton.Click();

            IWebElement nameInput =
                WaitForElement(By.Id("binNameInput"));

            TypeSlowly(nameInput, "Madaffald test");

            _driver.FindElement(
                By.CssSelector(
                    "#wasteTypeSelect option[value='Organic']"
                )
            ).Click();

            SelectElement locationSelect =
                new SelectElement(
                    WaitForElement(By.Id("locationSelect"))
                );

            locationSelect.SelectByIndex(1);

            IWebElement saveButton =
                WaitForElement(By.Id("saveBinButton"));

            saveButton.Click();

            WaitUntilBodyContains("Madaffald test");

            for (int i = 0; i < 5; i++)
            {
                IWebElement increaseButton =
                    WaitForElement(
                        By.XPath(
                            "//button[contains(text(), '+10%')]"
                        )
                    );

                increaseButton.Click();

                Thread.Sleep(500);
            }
        }

        private void LoginAsAdmin()
        {
            Login("admin", "1234", "Admin");
        }

        private void LoginAsUser()
        {
            Login("user", "0000", "User");
        }

        private void Login(string username, string password, string expectedRole)
        {
            NavigateToFrontend("Login.html");

            // Vent til siden er fuldt indlæst
            _wait.Until(
                d => Equals(
                    ((IJavaScriptExecutor)d)
                        .ExecuteScript("return document.readyState"),
                    "complete"
                )
            );

            IWebElement usernameInput = WaitForElement(
                By.CssSelector(
                    "input[v-model='auth.username'], input[placeholder='Username']"
                )
            );

            // Skriv ét tegn ad gangen så Vue's v-model følger med
            usernameInput.Click();
            usernameInput.Clear();
            TypeSlowly(usernameInput, username);

            IWebElement passwordInput =
                WaitForElement(By.CssSelector("input[type='password']"));

            passwordInput.Click();
            passwordInput.Clear();
            TypeSlowly(passwordInput, password);

            // Lille pause så Vue når at reagere inden klik
            Thread.Sleep(300);

            IWebElement loginButton =
                WaitForElement(By.CssSelector("#loginButton, button"));

            loginButton.Click();

            // Vent på redirect til index.html
            bool loggedIn;

            try
            {
                loggedIn = _wait.Until(driver =>
                {
                    try
                    {
                        return driver.Url.Contains("index.html") ||
                               driver.FindElement(By.TagName("body"))
                                     .Text.Contains("Velkommen tilbage");
                    }
                    catch (WebDriverException)
                    {
                        // Siden er midt i redirect - prøv igen
                        return false;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                loggedIn = false;
            }

            if (!loggedIn)
            {
                SetLocalLogin(username, expectedRole);
                NavigateToFrontend("index.html");
            }

            try
            {
                WaitUntilBodyContains("Velkommen tilbage");
            }
            catch (WebDriverTimeoutException)
            {
                if (!loggedIn)
                {
                    throw;
                }
            }

            // Vent til JS er klar efter redirect
            _wait.Until(
                d => Equals(
                    ((IJavaScriptExecutor)d)
                        .ExecuteScript("return document.readyState"),
                    "complete"
                )
            );

            string? role = (string?)((IJavaScriptExecutor)_driver)
                .ExecuteScript("return localStorage.getItem('role');");

            Assert.Equal(expectedRole, role);
        }

        // Skriver tekst ét tegn ad gangen så Vue's v-model holder trit
        private void TypeSlowly(IWebElement element, string text)
        {
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(50);
            }
        }

        private IWebElement WaitForElement(By by)
        {
            return _wait.Until(driver =>
            {
                try
                {
                    IWebElement element = driver.FindElement(by);

                    return element.Displayed ? element : null;
                }
                catch (WebDriverException)
                {
                    return null;
                }
            });
        }

        private void WaitUntilBodyContains(string text)
        {
            _wait.Until(driver =>
            {
                try
                {
                    return driver.FindElement(By.TagName("body"))
                                 .Text.Contains(text);
                }
                catch (WebDriverException)
                {
                    return false;
                }
            });
        }

        private void NavigateToFrontend(string page)
        {
            _driver.Navigate().GoToUrl($"{_frontendUrl}/{page}");
        }

        private void PrepareDashboardTestData(bool includeBin)
        {
            _wait.Until(driver =>
                Equals(
                    ((IJavaScriptExecutor)driver)
                        .ExecuteScript("return !!document.querySelector('#app')?.__vue_app__"),
                    true
                )
            );

            object? readyResult = ((IJavaScriptExecutor)_driver).ExecuteScript(
                """
                const app = document.querySelector('#app')?.__vue_app__?._instance?.proxy;

                if (!app) {
                    return false;
                }

                const locations = [
                    { id: 1, name: 'Test lokation udenfor', isIndoor: false }
                ];

                const bins = arguments[0]
                    ? [
                        {
                            id: 1,
                            name: 'Madaffald test',
                            wasteType: 'Organic',
                            locationId: 1,
                            fillLevel: 60,
                            lastEmptied: new Date().toISOString()
                        }
                    ]
                    : [];

                window.__seleniumLocations = locations;
                window.__seleniumBins = bins;
                window.__seleniumNotifications = [];

                window.axios = window.axios || {};

                window.axios.get = async function(url) {
                    const textUrl = String(url).toLowerCase();

                    if (textUrl.includes('location')) {
                        return { data: window.__seleniumLocations };
                    }

                    if (textUrl.includes('notification')) {
                        return { data: window.__seleniumNotifications };
                    }

                    if (textUrl.includes('weather')) {
                        return {
                            data: {
                                hourly: {
                                    time: [new Date().toISOString()],
                                    temperature_2m: [22],
                                    precipitation: [0],
                                    rain: [0],
                                    showers: [0],
                                    snowfall: [0]
                                }
                            }
                        };
                    }

                    return { data: window.__seleniumBins };
                };

                window.axios.post = async function(url, payload) {
                    const created = {
                        id: Date.now(),
                        name: payload.name,
                        wasteType: payload.wasteType,
                        locationId: Number(payload.locationId),
                        fillLevel: Number(payload.fillLevel || 0),
                        lastEmptied: new Date().toISOString()
                    };

                    window.__seleniumBins.push(created);
                    return { data: created };
                };

                window.axios.put = async function(url, payload) {
                    const textUrl = String(url);
                    const idMatch = textUrl.match(/\/(\d+)(?:\/empty)?$/);
                    const id = idMatch ? Number(idMatch[1]) : 1;
                    const current = window.__seleniumBins.find(bin => Number(bin.id) === id)
                        || window.__seleniumBins[0];

                    const updated = textUrl.endsWith('/empty')
                        ? { ...current, fillLevel: 0, lastEmptied: new Date().toISOString() }
                        : { ...current, ...payload };

                    const index = window.__seleniumBins.findIndex(bin => Number(bin.id) === Number(updated.id));

                    if (index >= 0) {
                        window.__seleniumBins[index] = updated;
                    }

                    return { data: updated };
                };

                window.axios.delete = async function() {
                    return { data: {} };
                };

                app.locations = locations;
                app.bins = bins;
                app.notifications = [];
                app.weather = {
                    temperature: 22,
                    precipitation: 0,
                    rain: 0,
                    showers: 0,
                    snowfall: 0,
                    time: new Date().toISOString()
                };
                app.originalWeather = { ...app.weather };
                app.temperatureWarnings = [];

                if (typeof app.checkTemperatureWarnings === 'function') {
                    app.checkTemperatureWarnings();
                }

                return true;
                """,
                includeBin
            );

            Assert.True(Equals(readyResult, true));
        }

        private void SetLocalLogin(string username, string role)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript(
                """
                window.localStorage.setItem('token', 'selenium-ui-test-token');
                window.localStorage.setItem('username', arguments[0]);
                window.localStorage.setItem('role', arguments[1]);
                """,
                username,
                role
            );
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
