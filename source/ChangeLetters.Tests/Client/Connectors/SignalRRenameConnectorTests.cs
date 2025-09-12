using ChangeLetters.Client.Connectors;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ChangeLetters.Tests.Client.Connectors;

[TestFixture]
public class SignalRRenameConnectorTests
{
    private IWebAssemblyHostEnvironment _hostEnvironment;
    private SignalRRenameConnector _connector;

    [SetUp]
    public void SetUp()
    {
        _hostEnvironment = Substitute.For<IWebAssemblyHostEnvironment>();
        _hostEnvironment.BaseAddress.Returns("https://localhost/");
        _connector = new SignalRRenameConnector(_hostEnvironment);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _connector.DisposeAsync().AsTask();
    }

    [Test]
    public async Task ConnectAsync_ShouldReturnConnectionId()
    {
        var token = CancellationToken.None;
        var connectionId = await _connector.ConnectAsync(token);
        connectionId.ShouldNotBeNullOrEmpty();
    }

    [Test]
    public async Task CloseAsync_ShouldNotThrow()
    {
        await _connector.ConnectAsync(CancellationToken.None);
        await Should.NotThrowAsync(() => _connector.CloseAsync());
    }

    [Test]
    public async Task DisposeAsync_ShouldNotThrow()
    {
        await _connector.ConnectAsync(CancellationToken.None);
        await Should.NotThrowAsync(() => _connector.DisposeAsync().AsTask());
    }

    [Test]
    public async Task ConnectionIdChanged_Event_ShouldBeRaised()
    {
        string receivedId = null;
        _connector.ConnectionIdChanged += id => receivedId = id;
        await _connector.ConnectAsync(CancellationToken.None);
        receivedId.ShouldNotBeNullOrEmpty();
    }
}
