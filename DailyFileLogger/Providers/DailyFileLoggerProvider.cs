using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DailyFileLogger.Core;
using QueueLibrary.Core;

namespace DailyFileLogger.Providers
{
    /// <summary>
    /// 負責產生 DailyFileLogger 的 Provider
    /// </summary>
    public class DailyFileLoggerProvider : ILoggerProvider
    {
        private readonly IHostEnvironment _env;
        private readonly OperationQueue _queue = new();

        public DailyFileLoggerProvider(IHostEnvironment env)
        {
            _env = env;
        }

        public ILogger CreateLogger(string categoryName)
            => new DailyFileLoggers(categoryName, _env, _queue);

        public void Dispose()
        {
            _queue.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}
