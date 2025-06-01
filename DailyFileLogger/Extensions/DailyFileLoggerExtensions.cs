using DailyFileLogger.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DailyFileLogger.Extensions
{
    /// <summary>
    /// 靜態擴充方法：讓使用端只要呼叫 builder.Logging.AddDailyFileLogger() 即可
    /// </summary>
    public static class DailyFileLoggerExtensions
    {
        /// <summary>
        /// 把 DailyFileLoggerProvider 加到 Logging pipeline
        /// </summary>
        public static ILoggingBuilder AddDailyFileLogger(this ILoggingBuilder builder)
        {
            // 1. 取出目前的 IHostEnvironment (透過 DI)
            var provider = builder.Services.BuildServiceProvider();
            var env = provider.GetService<IHostEnvironment>();

            // 2. 把自訂 Provider 註冊到 ILoggingBuilder
            builder.AddProvider(new DailyFileLoggerProvider(env));
            return builder;
        }
    }
}
