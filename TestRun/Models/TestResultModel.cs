using System;

namespace AutomationHelpers3.TestRun.Models
{
    public class TestResultModel
    {
        public int TestResultKey { get; set; }
        public int TestRunKey { get; set; }
        public string ValidationKey { get; set; }
        public string ValidationValue { get; set; }
        public string ResultStatus { get; set; }
        public string ExtraData { get; set; }
        public DateTime? TimeStamp { get; set; }


        public TestResultModel()
        {
            TestRunKey = Initialize.TestRunKey;
            TimeStamp = DateTime.Now;
        }
    }
}
