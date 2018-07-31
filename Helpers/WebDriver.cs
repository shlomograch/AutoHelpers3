using System;
using AutomationHelpers3.TestRun.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace AutomationHelpers3.Helpers
{
    public class WebDriver
    {
        public IWebDriver GetWebDriver(Preferences preferences)
        {
            switch (preferences.BrowserName.ToUpper())
            {

                case "IE":
                    {
                        InternetExplorerOptions ieOptions = new InternetExplorerOptions
                        {
                            EnsureCleanSession = true,
                            IgnoreZoomLevel = true
                        };
                        ieOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);

                        return new InternetExplorerDriver(ieOptions);
                    }
                case "CHROME":
                    {
                        SafariOptions options = new SafariOptions();
                        //ChromeOptions options = new ChromeOptions();
                        //options.SetLoggingPreference(LogType.Browser, preferences.BrowserConsoleLog ? LogLevel.All : LogLevel.Off);

                        return preferences.RemoteDriver ? new RemoteWebDriver(new Uri("http://10.217.34.66:4444/wd/hub"), options.ToCapabilities()) : new SafariDriver(options);
                    }
                case "FIREFOX":
                    {
                        return new FirefoxDriver();
                    }
                default:
                    {
                        throw new Exception("Invalid Browser Selected");
                    }
            }
        }
    }
}
