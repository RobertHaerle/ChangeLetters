using ChangeLetters.IO;
using ChangeLetters.DTOs;

namespace ChangeLetters.Tests.Server.IO;

[TestFixture]
public class ConfigurationIoTests
{
    private IJsonIo _jsonIoMock = null!;
    private ConfigurationIo _sut = null!;
    private ILogger<ConfigurationIo> _loggerMock = null!;
    private IEncryptionService _encryptionServiceMock = null!;

    [SetUp]
    public void SetUp()
    {
        _jsonIoMock = Substitute.For<IJsonIo>();
        _loggerMock = Substitute.For<ILogger<ConfigurationIo>>();
        _encryptionServiceMock = Substitute.For<IEncryptionService>();
        _sut = new ConfigurationIo(_jsonIoMock, _loggerMock, _encryptionServiceMock);
    }

    [Test]
    public void GetConfiguration_ReturnsDecryptedConfig()
    {
        var encryptedConfig = new EncryptedConfiguration
        {
            Port = 21,
            HostName = "host",
            UserName = "user",
            EncryptedPassword = "encrypted"
        };
        _jsonIoMock.Load<EncryptedConfiguration>(Arg.Any<string>()).Returns(encryptedConfig);
        _encryptionServiceMock.DecryptPassword("encrypted").Returns("decrypted");

        var result = _sut.GetConfiguration();

        result.Port.ShouldBe(21);
        result.HostName.ShouldBe("host");
        result.UserName.ShouldBe("user");
        result.Password.ShouldBe("decrypted");
    }

    [Test]
    public void SaveConfiguration_SavesEncryptedConfig()
    {
        var config = new Configuration
        {
            Port = 22,
            HostName = "host2",
            UserName = "user2",
            Password = "plain"
        };
        _encryptionServiceMock.EncryptPassword("plain").Returns("encrypted2");
        EncryptedConfiguration savedConfig = null;
        _jsonIoMock
            .When(x => x.Save(Arg.Any<string>(), Arg.Any<EncryptedConfiguration>()))
            .Do(call => savedConfig = (EncryptedConfiguration)call.Args()[1]);

        _sut.SaveConfiguration(config);

        savedConfig.ShouldNotBeNull();
        savedConfig!.Port.ShouldBe(22);
        savedConfig.HostName.ShouldBe("host2");
        savedConfig.UserName.ShouldBe("user2");
        savedConfig.EncryptedPassword.ShouldBe("encrypted2");
    }
}
