using ChangeLetters.Shared;

namespace ChangeLetters.Domain.IO;

/// <summary> 
/// Class ConfigurationIo.
/// Implements <see cref="IConfigurationIo" />
/// </summary>
[Export(typeof(IConfigurationIo))]
public class ConfigurationIo : IConfigurationIo
{
    private readonly IJsonIo _jsonIo;
    private readonly object _lock = new();
    private readonly ILogger<ConfigurationIo> _log;
    private readonly IEncryptionService _encryptionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationIo"/> class.
    /// </summary>
    /// <param name="jsonIo">The json io.</param>
    /// <param name="log">The log.</param>
    /// <param name="encryptionService">The encryption service.</param>
    public ConfigurationIo(
        IJsonIo jsonIo,
        ILogger<ConfigurationIo> log,
        IEncryptionService encryptionService)
    {
        _log = log;
        _jsonIo = jsonIo;
        _encryptionService = encryptionService;
    }

    /// <inheritdoc />
    public Configuration GetConfiguration()
    {
        lock (_lock)
        {
            try
            {
                var encryptedConfig = _jsonIo.Load<EncryptedConfiguration>("Configuration.json");
                var config = new Configuration
                {
                    Port = encryptedConfig.Port,
                    HostName = encryptedConfig.HostName,
                    UserName = encryptedConfig.UserName,
                    Password = _encryptionService.DecryptPassword(encryptedConfig.EncryptedPassword)
                };

                return config;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "");
                throw;
            }
        }
    }

    /// <inheritdoc />
    public void SaveConfiguration(Configuration configuration)
    {
        lock (_lock)
        {
            try
            {
                var encryptedConfig = new EncryptedConfiguration
                {
                    Port = configuration.Port,
                    HostName = configuration.HostName,
                    UserName = configuration.UserName,
                    EncryptedPassword = _encryptionService.EncryptPassword(configuration.Password)
                };
                _jsonIo.Save("Configuration.json", encryptedConfig);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "");
                throw;
            }
        }
    }
}