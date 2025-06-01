using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DailyFileLogger.Core;

namespace DailyFileLogger.Providers
{
    /// <summary>
    /// 負責產生 DailyFileLogger 的 Provider
    /// </summary>
    public class DailyFileLoggerProvider : ILoggerProvider
    {
        private readonly IHostEnvironment _env;

        public DailyFileLoggerProvider(IHostEnvironment env)
        {
            _env = env;
        }

        public ILogger CreateLogger(string categoryName)
            => new DailyFileLoggers(categoryName, _env);

        public void Dispose()
        {
            // 沒有要釋放的資源
        }
    }
}
