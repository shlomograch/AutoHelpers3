using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using OpenQA.Selenium;

namespace AutomationHelpers3.TestRun.Models
{
    public class TestRunModel
    {
        public int TestRunKey { get; set; }

        public string TestRunGuid { get; set; }

        public int TestKey { get; set; }

        public int TesterKey { get; set; }

        public int ApplicationKey { get; set; }

        public string Environment { get; set; }

        public string EnvironmentUrl { get; set; }

        public string Browser { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string TestRunStatus { get; set; }

        [JsonIgnore]
        public static IWebDriver Driver { get; set; }

        [JsonIgnore]
        public static List<TestResultModel> Results { get; set; }

        [JsonIgnore]
        public static bool EndToEndTest { get; set; }

        [JsonIgnore]
        public static bool IsApiTest { get; set; }

        public TestRunModel()
        {
            StartDateTime = DateTime.Now;
            Results = new List<TestResultModel>();
        }

    }
}
