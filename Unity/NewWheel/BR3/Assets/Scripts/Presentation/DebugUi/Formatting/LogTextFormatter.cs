namespace BR3.Presentation.DebugUi
{
    public static class LogTextFormatter
    {
        public static LogEntryViewData Format(string logLine)
        {
            return new LogEntryViewData
            {
                LogText = string.IsNullOrWhiteSpace(logLine) ? "-" : SlotIndexDisplayFormatter.NormalizeVisibleSlotNumbers(logLine),
            };
        }
    }
}
