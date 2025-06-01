namespace Logger
{
    /// <summary>
    /// 這個 ILogger 會把每一筆 LogMessage 寫到
    ///   {ContentRoot}/wwwroot/Log/yyyy/MM/dd.log
    /// </summary>
    public class DailyFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IHostEnvironment _hostEnv;
        private static readonly object _fileLock = new object();

        public DailyFileLogger(string categoryName, IHostEnvironment hostEnv)
        {
            _categoryName = categoryName;
            _hostEnv = hostEnv;
        }

        // 這個範例不支援 Scope
        public IDisposable BeginScope<TState>(TState state) => null;

        // 只要不是 None 就把它當作 Enabled
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        // 真正寫入檔案的方法
        public void Log<TState>(LogLevel logLevel,
                                EventId eventId,
                                TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            // 組出最終要寫的字串
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var message = formatter(state, exception);
            var exceptionText = exception != null
                ? $"  Exception: {exception.GetType().FullName} → {exception.Message}\n{exception.StackTrace}"
                : string.Empty;

            var logRecord = $"{timestamp} [{logLevel}] {_categoryName}: {message}{(exception != null ? "\n" + exceptionText : "")}";

            // 決定要存放的資料夾與檔案名稱
            // ContentRootPath 通常就是專案根目錄 (包含 wwwroot)
            var contentRoot = _hostEnv.ContentRootPath;
            var logDir = Path.Combine(contentRoot, "wwwroot", "Log",
                                 DateTime.Now.ToString("yyyy"),
                                 DateTime.Now.ToString("MM"));
            var logFilePath = Path.Combine(logDir, DateTime.Now.ToString("dd") + ".log");

            // 確保資料夾存在
            lock (_fileLock)
            {
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                // AppendAllText 每次自動以 UTF-8 (不帶 BOM) 附加
                File.AppendAllText(logFilePath, logRecord + Environment.NewLine);
            }
        }
    }

}
