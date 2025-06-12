using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueLibrary.Core;

namespace DailyFileLogger.Core
{
    /// <summary>
    /// 將 ILogger 訊息寫入至 wwwroot/Log/yyyy-MM/yyyy-MM-dd.log 的實作。
    /// </summary>
    public class DailyFileLoggers : ILogger
    {
        private readonly string _categoryName;
        private readonly IHostEnvironment _env;
        private readonly OperationQueue _queue;

        public DailyFileLoggers(string categoryName, IHostEnvironment env, OperationQueue queue)
        {
            _categoryName = categoryName;
            _env = env;
            _queue = queue;
        }

        /// <summary>
        /// 開始一個邏輯操作範圍。此實作不支援 Scope，始終回傳 <c>null</c>。
        /// </summary>
        /// <typeparam name="TState">Scope 的狀態型別。</typeparam>
        /// <param name="state">Scope 的識別狀態。</param>
        /// <returns>
        /// 回傳用於結束該邏輯操作範圍的 <see cref="IDisposable"/>。此實作無作用，直接回傳 <c>null</c>。
        /// </returns>
        public IDisposable BeginScope<TState>(TState state) => null;
        /// <summary>
        /// 檢查指定的 <see cref="LogLevel"/> 是否啟用。除了 <see cref="LogLevel.None"/> 之外皆回傳 <c>true</c>。
        /// </summary>
        /// <param name="logLevel">要檢查的記錄層級。</param>
        /// <returns>
        /// 如果指定的 <paramref name="logLevel"/> 可用，則回傳 <c>true</c>；否則回傳 <c>false</c>。
        /// </returns>
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        /// <summary>
        /// 將日誌條目寫入至每日對應的檔案中，檔案路徑為 {ContentRoot}/wwwroot/Log/yyyy/MM/dd.log。
        /// </summary>
        /// <typeparam name="TState">要寫入的狀態資料型別。</typeparam>
        /// <param name="logLevel">指定要寫入的日誌層級。</param>
        /// <param name="eventId">與此日誌條目相關聯的事件識別碼。</param>
        /// <param name="state">要寫入的狀態資料，可為任何物件或訊息。</param>
        /// <param name="exception">與此日誌條目相關的例外，若無則為 <c>null</c>。</param>
        /// <param name="formatter">
        /// 用來將 <paramref name="state"/> 與 <paramref name="exception"/> 格式化為 <see cref="string"/> 的委派函式。
        /// </param>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var message = formatter(state, exception);
            var exceptionText = exception != null
                ? $"  Exception: {exception.GetType().FullName} → {exception.Message}\n{exception.StackTrace}"
                : string.Empty;

            // 最終要寫入的字串
            var logLine = $"{timestamp} [{logLevel}] {_categoryName}: {message}"
                        + (exception != null ? "\n" + exceptionText : "");

            // 決定檔案路徑：{ContentRoot}/wwwroot/Log/yyyy/MM/dd.log
            var contentRoot = _env.ContentRootPath;
            var logDir = Path.Combine(
                contentRoot,
                "wwwroot",
                "Log",
                DateTime.Now.ToString("yyyy-MM")    
            );
            var logFile = Path.Combine(logDir, DateTime.Now.ToString("yyyy-MM-dd") + ".log");

            _ = _queue.EnqueueAsync(async () =>
            {
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                await File.AppendAllTextAsync(logFile, logLine + Environment.NewLine);
            });
        }
    }
}
