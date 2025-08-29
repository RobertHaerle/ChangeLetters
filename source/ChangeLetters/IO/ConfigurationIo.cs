using ChangeLetters.DTOs;
using ChangeLetters.Model;

namespace ChangeLetters.IO;

[Export(typeof(IConfigurationIo))]
public class ConfigurationIo : IConfigurationIo
{
    private readonly IJsonIo _jsonIo;
    private readonly object _lock = new();
    private readonly ILogger<ConfigurationIo> _log;
    private readonly IEncryptionService _encryptionService;

    public ConfigurationIo(
        IJsonIo jsonIo,
        ILogger<ConfigurationIo> log,
        IEncryptionService encryptionService)
    {
        _log = log;
        _jsonIo = jsonIo;
        _encryptionService = encryptionService;
        _log.LogInformation("instantiated {type}", GetType().Name);
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