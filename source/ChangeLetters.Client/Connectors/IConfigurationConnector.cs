using ChangeLetters.Shared;

namespace ChangeLetters.Client.Connectors;

/// <summary>Interface IConfigurationConnector.</summary>
public interface IConfigurationConnector
{
    /// <summary>Get configuration as an asynchronous operation.</summary>
    /// <returns>See description.</returns>
    Task<Configuration?> GetConfigurationAsync();

    /// <summary>Save configuration as an asynchronous operation.</summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>See description.</returns>
    Task SaveConfigurationAsync(Configuration configuration);
}