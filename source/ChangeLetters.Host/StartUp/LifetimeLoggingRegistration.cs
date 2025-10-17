namespace ChangeLetters.StartUp;


/// <summary> 
/// Class LifetimeLoggingRegistration. 
/// </summary>
public static class LifetimeLoggingRegistration
{
    /// <summary>Adds the lifetime logging.</summary>
    /// <param name="services">The services.</param>
    /// <returns>See description.</returns>
    public static IServiceProvider AddLifetimeLogging(this IServiceProvider services)
    {
        var lifetime = services.GetRequiredService<IHostApplicationLifetime>();
        var logger = services.GetRequiredService<ILogger<global::Program>>(); // Fully qualify Program

        lifetime.ApplicationStopped.Register(() => logger.LogInformation("Application has stopped."));
        lifetime.ApplicationStopping.Register(() => logger.LogInformation("Application is stopping."));
        
        return services;
    }
}
