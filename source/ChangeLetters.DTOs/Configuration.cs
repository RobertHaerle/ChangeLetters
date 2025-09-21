namespace ChangeLetters.DTOs;

/// <summary> 
/// Class ConfigurationBase. 
/// </summary>
public abstract class ConfigurationBase
{
    /// <summary>Gets or sets the name of the host.</summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>Gets or sets the port.</summary>
    public int Port { get; set; }

    /// <summary>Gets or sets the name of the user.</summary>
    public string UserName { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
        => $"HostName: {HostName} Port: {Port} UserName: {UserName}";
}

/// <summary> 
/// Class Configuration.
/// Inherits from <see cref="ConfigurationBase" />
/// </summary>
public class Configuration : ConfigurationBase
{
    /// <summary>Gets or sets the password.</summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary> 
/// Class EncryptedConfiguration.
/// Inherits from <see cref="ConfigurationBase" />
/// </summary>
public class EncryptedConfiguration : ConfigurationBase
{
    /// <summary>Gets or sets the encrypted password.</summary>
    public string EncryptedPassword { get; set; } = string.Empty;
}


