using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using AutomationHelpers3.Helpers;
using AutomationHelpers3.TestRun.Models;
using Newtonsoft.Json;
using NewtonSoftSerializer = Newtonsoft.Json.JsonSerializer;

namespace AutomationHelpers3.TestRun
{
    public class Initialize
    {
        public static int TestRunKey { get; set; }

        public Initialize() { }

        public Initialize(string appName, out TestRunModel testRun)
        {
            try
            {
                Api prefs;
                Preferences preferences;

                if (Dns.GetHostName() == "VDI-Automation0")
                {
                    // Get Queued User Prefs
                    prefs = new Api($"api/GetPreferences/{appName}/{Environment.UserName}/{true}");
                    JsonReader reader = new JsonTextReader(new StringReader(prefs.GetApi().Content));
                    preferences = new NewtonSoftSerializer().Deserialize<Preferences>(reader);
                }
                else
                {
                    // Get Normal User Prefs
                    prefs = new Api($"api/GetPreferences/{appName}/{Environment.UserName}/{false}");
                    JsonReader reader = new JsonTextReader(new StringReader(prefs.GetApi().Content));
                    preferences = new NewtonSoftSerializer().Deserialize<Preferences>(reader);
                    preferences.TesterName = Environment.UserName;
                }
                //preferences.RemoteDriver = false;

                var appKeyRequest = new Api($"api/GetAppKey/{appName}");
                var appKey = int.Parse(appKeyRequest.GetApi().Content);
                Debug.WriteLine($"Initializing Application: {appName} - Key: {appKey}");

                var testName = "";
                Debug.WriteLine("Initializing Test: " + GetTestName());
                var testKey = new Api($"api/GetTestKey/{appName}/{GetTestName()}");
                var testKeyContent = testKey.GetApi().Content;
                if (testKeyContent.Contains("Test name not found"))
                {
                    Test newTest = new Test
                    {
                        ApplicationKey = appKey,
                        Active = true,
                        TestName = GetTestName()
                    };
                    testName = newTest.TestName;

                    Debug.WriteLine($"{GetTestName()} not found.. Creating new test.");
                    // Do some logic to create a new test for this test name.
                    var createTestRequest = new Api("api/CreateTest");
                    testKeyContent = createTestRequest.PostApi(JsonConvert.SerializeObject(newTest)).Content; ; // Set key to newly created test key

                    Debug.WriteLine($"Initializing Test: {GetTestName()} - Key: {testKeyContent}");
                }

                testRun = new TestRunModel
                {
                    ApplicationKey = appKey,
                    TestRunStatus = RunStatus.Status.Initializing.ToString(),
                    Environment = preferences.Environment,
                    EnvironmentUrl = preferences.EnvironmentUrl,
                    TestKey = int.Parse(testKeyContent),
                    TesterKey = preferences.TesterKey,
                    Browser = preferences.BrowserName,
                };

                var createTestRunKey = new Api("api/CreateTestRun");
                testRun.TestRunKey = int.Parse(createTestRunKey.PostApi(JsonConvert.SerializeObject(testRun)).Content);
                TestRunKey = testRun.TestRunKey;

                if (testName != string.Empty && !testName.ToUpper().Contains("API"))
                {
                    WebDriver driverData = new WebDriver();
                    TestRunModel.Driver = driverData.GetWebDriver(preferences);
                    Debug.WriteLine("Started Driver type: " + TestRunModel.Driver.GetType());

                    TestRunModel.Driver.Manage().Window.Maximize();

                    TestRunModel.Driver.Navigate().GoToUrl(testRun.EnvironmentUrl);
                    Debug.WriteLine($"Went to URL: {TestRunModel.Driver.Url}");
                }
                else
                {
                    TestRunModel.IsApiTest = true;
                    Debug.WriteLine("Started API Test.");
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                throw;
            }

        }
        public string GetTestName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(3);

            return sf.GetMethod().Name;
        }
    }
}
