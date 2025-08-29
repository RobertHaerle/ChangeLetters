using FluentFTP;

namespace ChangeLetters.StartUp;

internal static class FluentFtpRegistration
{
    internal static IServiceCollection AddFluentFtpComponents(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<AsyncFtpClient>();
        serviceCollection.AddTransient<FluentFTP.Logging.FtpLogAdapter>(services =>
        {
            var logger = services.GetRequiredService<ILogger<AsyncFtpClient>>();
            return new FluentFTP.Logging.FtpLogAdapter(logger);
        });
        return serviceCollection;
    }
}
