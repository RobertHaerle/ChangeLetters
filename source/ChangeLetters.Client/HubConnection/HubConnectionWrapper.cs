using Microsoft.AspNetCore.SignalR.Client;

namespace ChangeLetters.Client.HubConnection;

/// <summary> 
/// Class HubConnectionWrapper.
/// Implements <see cref="IHubConnection" />
/// </summary>
public class HubConnectionWrapper : IHubConnection
{
    private readonly Microsoft.AspNetCore.SignalR.Client.HubConnection _hubConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="HubConnectionWrapper"/> class.
    /// </summary>
    /// <param name="hubConnection">The hub connection.</param>
    public HubConnectionWrapper(Microsoft.AspNetCore.SignalR.Client.HubConnection hubConnection)
    {
        _hubConnection = hubConnection;
    }

    /// <inheritdoc />
    public string? ConnectionId => _hubConnection.ConnectionId;

    /// <inheritdoc />
    public HubConnectionState State => _hubConnection.State;

    /// <inheritdoc />
    public event Func<Exception?, Task>? Closed
    {
        add => _hubConnection.Closed += value;
        remove => _hubConnection.Closed -= value;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default)
        => _hubConnection.StartAsync(cancellationToken);

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = default)
        => _hubConnection.StopAsync(cancellationToken);

    /// <inheritdoc />
    public IDisposable On<T>(string methodName, Action<T> handler)
        => _hubConnection.On(methodName, handler);

    /// <inheritdoc />
    public ValueTask DisposeAsync()
        => _hubConnection.DisposeAsync();
}
