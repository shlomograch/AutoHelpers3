namespace AutomationHelpers3.TestRun.Models
{
    public class Test
    {
        public int TestListKey { get; set; }

        public string TestName { get; set; }

        public string TestDescription { get; set; }

        public int ApplicationKey { get; set; }

        public bool? Active { get; set; }

        public int? TotalIterations { get; set; }

        public int? ManualTime { get; set; }

        public int? AverageAutomatedTime { get; set; }
    }
}
