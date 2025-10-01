using Flurl;
using ChangeLetters.Client.Connectors;
using ChangeLetters.Client.HubConnection;
using ChangeLetters.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ChangeLetters.Tests.Client.Connectors;

[TestFixture]
public class SignalRRenameConnectorTests
{
    private SignalRRenameConnector _sut;
    private IHubConnection _mockHubConnection;
    private IHubConnectionFactory _hubConnectionFactory;
    private IWebAssemblyHostEnvironment _hostEnvironment;

    [SetUp]
    public void SetUp()
    {
        _hostEnvironment = Substitute.For<IWebAssemblyHostEnvironment>();
        _hostEnvironment.BaseAddress.Returns("https://localhost/");
        _hubConnectionFactory = Substitute.For<IHubConnectionFactory>();
        _mockHubConnection = Substitute.For<IHubConnection>();
        _mockHubConnection.ConnectionId.Returns("test-connection-id");
        _mockHubConnection.State.Returns(HubConnectionState.Connected);

        _hubConnectionFactory.CreateConnection(Arg.Any<Url>())
            .Returns(_mockHubConnection);
        _sut = new SignalRRenameConnector(_hubConnectionFactory, _hostEnvironment);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _sut.DisposeAsync();
    }

    [Test]
    public async Task ConnectAsync_ShouldReturnConnectionId()
    {
        // Act
        var connectionId = await _sut.ConnectAsync(CancellationToken.None);

        // Assert
        connectionId.ShouldBe("test-connection-id");
        await _mockHubConnection.Received(1).StartAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ConnectAsync_ShouldRaiseConnectionIdChangedEvent()
    {
        // Arrange
        string receivedId = null;
        _sut.ConnectionIdChanged += id => receivedId = id;

        // Act
        await _sut.ConnectAsync(CancellationToken.None);

        // Assert
        receivedId.ShouldBe("test-connection-id");
    }

    [Test]
    public async Task ConnectAsync_ShouldRegisterEventHandlers()
    {
        // Act
        await _sut.ConnectAsync(CancellationToken.None);

        // Assert
        _mockHubConnection.Received(1).On<CurrentItemCount>(
            SignalRPath.Rename.CurrentItemCount,
            Arg.Any<Action<CurrentItemCount>>());
        _mockHubConnection.Received(1).On<CompleteItemCount>(
            SignalRPath.Rename.CompleteItemCount,
            Arg.Any<Action<CompleteItemCount>>());
    }

    [Test]
    public async Task CloseAsync_ShouldStopConnection()
    {
        // Arrange
        await _sut.ConnectAsync(CancellationToken.None);

        // Act
        await _sut.CloseAsync();

        // Assert
        await _mockHubConnection.Received(1).StopAsync();
    }

    [Test]
    public async Task CurrentItemCountChanged_ShouldBeTriggeredBySignalREvent()
    {
        // Arrange
        CurrentItemCount receivedCount = null;
        _sut.CurrentItemCountChanged += count => receivedCount = count;

        Action<CurrentItemCount> capturedHandler = null!;
        _mockHubConnection.On<CurrentItemCount>(
            SignalRPath.Rename.CurrentItemCount,
            Arg.Do<Action<CurrentItemCount>>(handler => capturedHandler = handler));

        await _sut.ConnectAsync(CancellationToken.None);

        var testCount = new CurrentItemCount(FileItemType.File, 5);

        // Act
        capturedHandler(testCount);

        // Assert
        receivedCount.ShouldNotBeNull();
        receivedCount.Current.ShouldBe(5);
        receivedCount.FileItemType.ShouldBe(FileItemType.File);
    }

    [Test]
    public async Task HubConnectionClosed_ShouldRaiseConnectionIdChangedWithEmptyString()
    {
        // Arrange
        string receivedId = null;
        _sut.ConnectionIdChanged += id => receivedId = id;

        Func<Exception, Task> capturedClosedHandler = null!;
        _mockHubConnection.Closed += Arg.Do<Func<Exception, Task>>(
            handler => capturedClosedHandler = handler);

        await _sut.ConnectAsync(CancellationToken.None);
        receivedId = null; // Reset nach dem Connect

        // Act
        await capturedClosedHandler(null);

        // Assert
        receivedId.ShouldBe(string.Empty);
    }
}
