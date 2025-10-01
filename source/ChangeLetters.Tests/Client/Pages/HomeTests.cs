using Bunit;
using ChangeLetters.Client.Pages;
using ChangeLetters.Client.Connectors;
using ChangeLetters.Shared;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Bunit.TestContext;

namespace ChangeLetters.Tests.Client.Pages;

public class HomeTests
{
    private TestContext _textContext;
    [SetUp]
    public void Setup()
    {   
        _textContext = new TestContext();
        var ftpConnector = Substitute.For<IFtpConnectorClient>();
        var configConnector = Substitute.For<IConfigurationConnector>();

        configConnector.GetConfigurationAsync().Returns(Task.FromResult(new Configuration
        {
            HostName = "localhost",
            Port = 21,
            UserName = "user",
            Password = "pass"
        }));

        ftpConnector.ConnectAsync(Arg.Any<Configuration>()).Returns(Task.FromResult(true));

        _textContext.Services.AddSingleton(ftpConnector);
        _textContext.Services.AddSingleton(configConnector);
    }

    [TearDown]
    public void TearDown()
    {
        _textContext.Dispose();
    }


    [Test]
    public void HomePage_ShouldRenderCorrectly()
    {
        // Arrange
        var component = _textContext.RenderComponent<Home>();
        // Act
        var header = component.Find("h1");
        // Assert
        header.MarkupMatches("<h1>Enter connection data</h1>");
    }

    [Test]
    public void ConnectButton_ShouldShowConnectedSuccessfullyMessage()
    {
        // Arrange
        var component = _textContext.RenderComponent<Home>();
        var button = component.Find("#save");

        // Act
        button.Click();

        // Assert
        var alert = component.Find(".alert-success");
        alert.MarkupMatches(@"<div class=""alert alert-success"" role=""alert""><strong>Connected successfully!</strong></div>");
    }
}
