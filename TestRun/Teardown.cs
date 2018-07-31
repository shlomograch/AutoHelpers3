using System;
using System.Diagnostics;

using AutomationHelpers3.Helpers;
using AutomationHelpers3.TestRun.Models;
using OpenQA.Selenium;
using RestSharp;

namespace AutomationHelpers3.TestRun
{
    public class Teardown : Runner
    {
        public RestResponse _TeardownData { get; }
        public Teardown(TestRunModel testRun)
        {
            Debug.WriteLine("Starting Teardown.");
            testRun.TestRunStatus = RunStatus.Status.Pass.ToString();

            testRun.EndDateTime = DateTime.Now;
            foreach (var item in TestRunModel.Results)
            {
                if (item.ResultStatus == RunStatus.Status.Fail.ToString() || item.ResultStatus == RunStatus.Status.Fatal.ToString())
                {
                    // TODO: This will only set the result status to fail, but will never show fatals/warnings ect.
                    // TODO: Consider making a prioritized list of which failure is highest pri (Pass - 1, Info - 2, Warning - 3, Fatal - 4, Critical - 5, ect).

                    testRun.TestRunStatus = RunStatus.Status.Fail.ToString();
                    break;
                }
            }

            if (TestRunModel.IsApiTest == false)
            {
                if (testRun.Browser.ToUpper() == "CHROME")
                {
                    Debug.WriteLine("Writing Chrome driver logs to end of test run.");
                    if (TestRunModel.Driver.Manage().Logs.GetLog(LogType.Browser) != null)
                    {
                        var logs = TestRunModel.Driver.Manage().Logs.GetLog(LogType.Browser);

                        for (int i = 0; i < logs.Count; i++)
                        {
                            if (logs.Count - 1 == i)
                            {
                                break;
                            }

                            TestResultModel result = new TestResultModel
                            {
                                ValidationKey = logs[i].Message,
                                ValidationValue = logs[i].Level.ToString(),
                                ResultStatus = RunStatus.Status.Warning.ToString()
                            };
                            WriteResult(result);
                        }
                    }
                }

                TestRunModel.Driver?.Quit();
                Debug.WriteLine($"{TestRunModel.Driver} closed.");
            }

            Api api = new Api("api/CreateTestRun");
            _TeardownData = api.UpdateTestRun(testRun);

        }
    }
}
