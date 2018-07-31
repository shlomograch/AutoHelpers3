using System;

namespace AutomationHelpers3.TestRun.Models
{
    public class Preferences
    {
        public int Id { get; set; }
        public string TesterName { get; set; }
        public int TesterKey { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationKey { get; set; }
        public string EnvironmentDescription { get; set; }
        public string Environment { get; set; }
        public string BrowserName { get; set; }
        public int BrowserId { get; set; }
        public bool? StackTrace { get; set; }
        public bool RemoteDriver { get; set; }
        public bool? Utilization { get; set; }
        public bool BrowserConsoleLog { get; set; }
        public DateTime? OccurrenceDate { get; set; }
        public string EnvironmentUrl { get; set; }
        public bool? WriteToDatabase { get; set; }
    }
}
