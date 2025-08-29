using Microsoft.AspNetCore.DataProtection;

namespace ChangeLetters.StartUp;

internal static class DataProtectionRegistration
{
    internal static IServiceCollection AddDataProtectionComponents(
        this IServiceCollection services, IConfiguration configuration)
    {
        var dataProtectionPath = configuration["DataDirectory"] ?? Path.Combine(AppContext.BaseDirectory, "../Data");
        Directory.CreateDirectory(dataProtectionPath);
        services.AddDataProtection()
            .SetApplicationName("ChangeLettersApp")
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
        return services;
    }
}
        