using FluentFTP;
using FluentFTP.Logging;

namespace ChangeLetters.StartUp;

internal static class FluentFtpRegistration
{
    internal static IServiceCollection AddFluentFtpComponents(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<AsyncFtpClient>();
        serviceCollection.AddTransient<FtpLogAdapter>(services =>
        {
            var logger = services.GetRequiredService<ILogger<AsyncFtpClient>>();
            return new FtpLogAdapter(logger);
        });
        return serviceCollection;
    }
}
