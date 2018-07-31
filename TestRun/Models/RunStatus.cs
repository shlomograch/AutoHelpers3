namespace AutomationHelpers3.TestRun.Models
{
    public class RunStatus
    {
        public enum Status
        {
            Pass,
            Fail,
            Fatal,
            Warning,
            Info,
            Skip,
            Running,
            Initializing
        }
    }
}
