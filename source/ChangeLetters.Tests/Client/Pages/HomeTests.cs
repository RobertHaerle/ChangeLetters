using Bunit;
using ChangeLetters.DTOs;
using ChangeLetters.Client.Pages;
using ChangeLetters.Client.Connectors;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Bunit.TestContext;

namespace ChangeLetters.Tests.Client.Pages;

public class HomeTests : TestContext
{
    [SetUp]
    public void Setup()
    {   
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

        Services.AddSingleton(ftpConnector);
        Services.AddSingleton(configConnector);
    }

    [Test]
    public void HomePage_ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<Home>();
        // Act
        var header = component.Find("h1");
        // Assert
        header.MarkupMatches("<h1>Enter connection data</h1>");
    }

    [Test]
    public void ConnectButton_ShouldShowConnectedSuccessfullyMessage()
    {
        // Arrange
        var component = RenderComponent<Home>();
        var button = component.Find("#save");

        // Act
        button.Click();

        // Assert
        var alert = component.Find(".alert-success");
        alert.MarkupMatches(@"<div class=""alert alert-success"" role=""alert""><strong>Connected successfully!</strong></div>");
    }
}
