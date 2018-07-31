using System.Net;
using AutomationHelpers3.Helpers;
using AutomationHelpers3.TestRun.Models;
using Newtonsoft.Json;

namespace AutomationHelpers3.TestRun
{
    public class Runner : Initialize
    {
        public TestRunModel StartTest(string appName)
        {
            new Initialize(appName, out TestRunModel testRun);
            testRun = NextStatus(testRun, RunStatus.Status.Running);

            // TODO: Create multi-threaded Test Runner so that you can log while running test.
            // TODO: Add Dependency Injection here so that We can call tear down inside of this method, and inject test code into this section.
            //testRun = NextStatus(testRun, RunStatus.Status.Skip);

            return testRun;
        }

        public TestRunModel NextStatus(TestRunModel testRun, RunStatus.Status status)
        {
            testRun.TestRunStatus = status.ToString();

            Api api = new Api("api/CreateTestRun");
            api.PostApi(JsonConvert.SerializeObject(testRun));

            return testRun;
        }

        public HttpStatusCode WriteResult(TestResultModel testResult)
        {
            TestRunModel.Results.Add(testResult);

            Api api = new Api("api/CreateTestResult");
            return api.PostApi(JsonConvert.SerializeObject(testResult)).StatusCode;
        }
    }
}
