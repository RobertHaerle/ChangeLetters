using ChangeLetters.DTOs;

namespace ChangeLetters.IO;

/// <summary>Interface IConfigurationIo.</summary>
public interface IConfigurationIo
{
    /// <summary>Gets the configuration.</summary>
    /// <returns>See description.</returns>
    Configuration GetConfiguration();

    /// <summary>Saves the configuration.</summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>See description.</returns>
    void SaveConfiguration(Configuration configuration);
}