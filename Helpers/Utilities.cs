using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AutomationHelpers3.TestRun;
using AutomationHelpers3.TestRun.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace AutomationHelpers3.Helpers
{
    public static class Utilities
    {
        public static void PageLoadWithWriting()
        {
            Runner runner = new Runner();

            Thread.Sleep(4000);
            PageLoad();
            waitForDocumentLoad();
            waitForAjaxLoad();
            waitForDocumentLoad();

            TestResultModel result = new TestResultModel
            {
                ValidationValue = TestRunModel.Driver.Title + " Page is loaded",
                ResultStatus = RunStatus.Status.Info.ToString()
            };
            runner.WriteResult(result);


        }


        internal static void waitForAjaxLoad(int timeoutSecs = 10)
        {
            try
            {
                for (var i = 0; i < timeoutSecs; i++)
                {
                    var ajaxIsComplete = (bool)(TestRunModel.Driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                    if (ajaxIsComplete)
                        return;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal static void waitForDocumentLoad(int timeoutSecs = 10)
        {
            try
            {
                for (var i = 0; i < timeoutSecs; i++)
                {
                    var ajaxIsComplete = (bool)(TestRunModel.Driver as IJavaScriptExecutor).ExecuteScript("return document.readyState")
                        .ToString().Equals("complete");
                    if (ajaxIsComplete)
                        return;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void PageLoad()
        {
            try
            {
                // Only works for MMA/FM sites
                //there is no need to have seperate columns for start and end time ... time stamp
                IWebElement loading = TestRunModel.Driver.FindElement(By.Id("loading-bar"));
                Stopwatch time = new Stopwatch();
                time.Start();

                while (loading.Displayed || time.ElapsedMilliseconds < 20000)
                {
                    loading = TestRunModel.Driver.FindElement(By.Id("loading-bar"));
                    Thread.Sleep(1000);
                    Console.WriteLine(time);
                }
                time.Stop();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no such element") || ex.Message.Contains("stale element"))
                {
                    Console.WriteLine($"{TestRunModel.Driver.Title} page is loaded");
                }
                else
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationValue = "Page isn't fully loaded",
                        ResultStatus = RunStatus.Status.Warning.ToString()
                    };
                    
                    Runner runner = new Runner();
                    runner.WriteResult(testResult);
                }
            }
        }

        #region Misc Methods

        public static void ClearAndEnterText(this IWebElement objectElement, string textToSend)
        {
            try
            {
                objectElement.Clear();
                objectElement.SendKeys(textToSend);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void ScrollToElement(this IWebElement element)
        {
            IJavaScriptExecutor js = TestRunModel.Driver as IJavaScriptExecutor;
            int x = element.Location.X;
            int y = 0;
            if (element.Location.Y > 100)
            {
                y = element.Location.Y - 100;
            }
            else
            {
                y = element.Location.Y;
            }
            js.ExecuteScript("window.scrollTo(" + x + "," + y + ")");
        }

        #endregion Misc Methods

        #region Dropdown Methods

        public static string GetDropDownListText(this IWebElement objectElement)
        {
            return new SelectElement(objectElement).AllSelectedOptions.SingleOrDefault().Text;
        }

        public static void SelectDropdownByText(this IWebElement objectElement, string text)
        {
            new SelectElement(objectElement).SelectByText(text);
        }

        #endregion Dropdown Methods

        #region Post Methods

        public static IWebElement PostInfoElementTextByXpath(string xpath, string act = null, int sec = 15)
        {
            Runner runner = new Runner();

            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(sec));
                var elementCurrent = wait.Until(x => x.FindElement(By.XPath(xpath)));

                if (act != null && !elementCurrent.Text.Contains(act))
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationKey = act,
                        ValidationValue = elementCurrent.Text,
                        ResultStatus = RunStatus.Status.Fail.ToString()
                    };

                    runner.WriteResult(testResult);
                }

                if (elementCurrent.Displayed)
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationKey = act,
                        ValidationValue = elementCurrent.Text,
                        ResultStatus = RunStatus.Status.Info.ToString()
                    };

                    runner.WriteResult(testResult);

                    return elementCurrent;
                }
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationKey = act,
                    ValidationValue = ex.Message,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }

            return null;
        }

        #endregion Post Methods

        #region Verify Methods

        public static IWebElement VerifyElementTextByXpath(string xpath, string expected, int sec = 20)
        {
            Runner runner = new Runner();
            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(sec));
                var elementCurrent = wait.Until(x => x.FindElement(By.XPath(xpath)));

                if (elementCurrent.Text.ToUpper().Contains(expected.ToUpper()) || elementCurrent.TagName.ToUpper().Contains(expected.ToUpper()))
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationKey = expected,
                        ValidationValue = elementCurrent.Text,
                        ResultStatus = RunStatus.Status.Pass.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                else
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationKey = expected,
                        ValidationValue = elementCurrent.Text,
                        ResultStatus = RunStatus.Status.Fail.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                return elementCurrent;
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationKey = expected,
                    ValidationValue = ex.Message,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }
            return null;
        }

        public static IWebElement VerifyElementTextByXpathAttribute(string xpath, string expected, int sec = 15)
        {
            Runner runner = new Runner();

            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(sec));
                var elementCurrent = wait.Until(x => x.FindElement(By.XPath(xpath)));

                if (elementCurrent.GetAttribute("href").ToUpper().Contains(expected.ToUpper()) || elementCurrent.TagName.ToUpper().Contains(expected.ToUpper()))
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationKey = expected,
                        ValidationValue = elementCurrent.GetAttribute("href"),
                        ResultStatus = RunStatus.Status.Pass.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                else
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationKey = expected,
                        ValidationValue = elementCurrent.GetAttribute("href"),
                        ResultStatus = RunStatus.Status.Fail.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                return elementCurrent;
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationKey = expected,
                    ValidationValue = ex.Message,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }
            return null;
        }

        #endregion Verify Methods

        #region Wait Methods

        public static IWebElement WaitAndFindElementByID(string Id)
        {
            Runner runner = new Runner();

            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(15));
                var webElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(Id)));

                TestResultModel testResult = new TestResultModel
                {
                    ValidationValue = webElement.Text,
                    ResultStatus = RunStatus.Status.Pass.ToString()
                };

                runner.WriteResult(testResult);

                return webElement;
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationValue = ex.Message,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }
            return null;
        }

        public static IWebElement WaitAndFindElementByID(string id, int sec = 15)
        {
            Runner runner = new Runner();

            IWebElement elementCurrent;
            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(sec));
                elementCurrent = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));

                if (elementCurrent.Displayed)
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationValue = id,
                        ResultStatus = RunStatus.Status.Pass.ToString()
                    };

                    runner.WriteResult(testResult);
                    new Actions(TestRunModel.Driver).MoveToElement(elementCurrent).Perform();
                }
                else
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationKey = id,
                        ValidationValue = elementCurrent.GetAttribute("outerHTML"),
                        ResultStatus = RunStatus.Status.Fail.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                return elementCurrent;
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationValue = id,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }
            return null;
        }

        public static IWebElement WaitAndFindElementByName(string name, string expected, int sec = 15)
        {
            Runner runner = new Runner();

            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(sec));
                var elementCurrent = wait.Until(ExpectedConditions.ElementIsVisible(By.Name(name)));

                if (elementCurrent.Displayed)
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationValue = elementCurrent.Text,
                        ResultStatus = RunStatus.Status.Pass.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                else
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationValue = expected,
                        ResultStatus = RunStatus.Status.Fail.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                return elementCurrent;
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationKey = expected,
                    ValidationValue = ex.Message,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }
            return null;
        }

        public static IWebElement WaitAndFindElementByXpath(string xpath)
        {
            Runner runner = new Runner();

            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(30));
                var webElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));

                TestResultModel testResult = new TestResultModel
                {
                    ValidationValue = webElement.Text,
                    ResultStatus = RunStatus.Status.Pass.ToString()
                };

                runner.WriteResult(testResult);

                return webElement;
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationValue = ex.Message,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }
            return null;
        }

        public static IWebElement WaitAndFindElementByXpath(string xpath, string expected, int sec = 15)
        {
            Runner runner = new Runner();

            try
            {
                WebDriverWait wait = new WebDriverWait(TestRunModel.Driver, TimeSpan.FromSeconds(sec));
                var elementCurrent = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));

                if (elementCurrent.Displayed)
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationValue = elementCurrent.Text,
                        ResultStatus = RunStatus.Status.Pass.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                else
                {
                    TestResultModel testResult = new TestResultModel
                    {
                        ValidationValue = expected,
                        ResultStatus = RunStatus.Status.Fail.ToString()
                    };

                    runner.WriteResult(testResult);
                }
                return elementCurrent;
            }
            catch (Exception ex)
            {
                TestResultModel testResult = new TestResultModel
                {
                    ValidationKey = expected,
                    ValidationValue = ex.Message,
                    ResultStatus = RunStatus.Status.Fatal.ToString()
                };

                runner.WriteResult(testResult);
            }
            return null;
        }

        #endregion Wait Methods

        public static void TakeScreenshot(this IWebElement element)
        {
            var jsDriver = (IJavaScriptExecutor)TestRunModel.Driver;
            string highlightJavascript =
                @"$(arguments[0]).css({ 'border-width' : '2px', 'border-style' : 'solid', 'border-color' : 'red' });";

            jsDriver.ExecuteScript(highlightJavascript, element);

            TestRunModel.Driver.TakeScreenshot()
                .SaveAsFile("Automation-Exception-Test" + Stopwatch.GetTimestamp(), ScreenshotImageFormat.Jpeg);
        }

        public static void GetUtilization()
        {
            Runner runner = new Runner();

            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            cpuCounter.NextValue();
            ramCounter.NextValue();

            Thread.Sleep(1000); // Wait one second to gather results for performance counter before getting the stats again.

            TestResultModel cpuresult = new TestResultModel
            {
                ValidationValue = cpuCounter.NextValue() + " %",
                ResultStatus = RunStatus.Status.Info.ToString()
            };
            
            TestResultModel ramresult = new TestResultModel
            {
                ValidationValue = ramCounter.NextValue() + " mb",
                ResultStatus = RunStatus.Status.Info.ToString()
            };
            
            runner.WriteResult(cpuresult);
            runner.WriteResult(ramresult);

        }
    }
}
