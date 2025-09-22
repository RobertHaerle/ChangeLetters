using Microsoft.Extensions.Logging;

namespace ChangeLetters.IntegrationTests.Helpers;

public static class LogHelper
{
    public static ILogger<T> GetLogger<T>() where T:class
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Trace);
        });
        return loggerFactory.CreateLogger<T>();
    }
}
