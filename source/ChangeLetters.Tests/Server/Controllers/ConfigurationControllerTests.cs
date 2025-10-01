using ChangeLetters.Application.Http.Controllers;
using ChangeLetters.Domain.IO;
using ChangeLetters.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ChangeLetters.Tests.Server.Controllers;

[TestFixture]
public class ConfigurationControllerTests
{
    private IConfigurationIo _configIo = null!;
    private ILogger<ConfigurationController> _logger = null!;
    private ConfigurationController _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _configIo = Substitute.For<IConfigurationIo>();
        _logger = Substitute.For<ILogger<ConfigurationController>>();
        _sut = new ConfigurationController(_configIo, _logger);
    }

    [Test]
    public void Get_ReturnsConfig()
    {
        var config = new Configuration();
        _configIo.GetConfiguration().Returns(config);
        var result = _sut.Get();
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(config);
    }

    [Test]
    public void Save_CallsSaveConfiguration_AndReturnsNoContent()
    {
        var config = new Configuration();
        var result = _sut.Save(config);
        _configIo.Received(1).SaveConfiguration(config);
        result.ShouldBeOfType<NoContentResult>();
    }
}
