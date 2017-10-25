namespace StockManager.Models
{
    public class LogEntry
    {
        public int Id { get; private set; }
        public string CallSite { get; private set; }
        public string Date { get; private set; }
        public string Exception { get; private set; }
        public string Level { get; private set; }
        public string Logger { get; private set; }
        public string MachineName { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }
        public string Thread { get; private set; }
        public string Username { get; private set; }
    }
}
