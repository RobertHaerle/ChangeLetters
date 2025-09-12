using Microsoft.AspNetCore.SignalR.Client;

namespace ChangeLetters.Client.HubConnection;

/// <summary>
/// Interface IHubConnection.
/// Extends the <see cref="System.IAsyncDisposable" />.
/// Wraps <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection"/> for easier testing and abstraction.
/// </summary>
public interface IHubConnection : IAsyncDisposable
{
    /// <summary>
    /// Gets the current connection ID assigned by the SignalR server.
    /// Corresponds to <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection.ConnectionId"/>.
    /// </summary>
    string? ConnectionId { get; }

    /// <summary>
    /// Gets the current state of the connection.
    /// Corresponds to <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection.State"/>.
    /// </summary>
    HubConnectionState State { get; }

    /// <summary>
    /// Occurs when the connection is closed.
    /// Corresponds to <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection.Closed"/>.
    /// </summary>
    event Func<Exception?, Task>? Closed;

    /// <summary>
    /// Starts the SignalR connection asynchronously.
    /// Corresponds to <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection.StartAsync(CancellationToken)"/>.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the SignalR connection asynchronously.
    /// Corresponds to <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection.StopAsync(CancellationToken)"/>.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a handler that will be invoked when the specified method is called from the server.
    /// Corresponds to <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnection.On{T}(string, Action{T})"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter expected from the server method.</typeparam>
    /// <param name="methodName">The name of the method to handle.</param>
    /// <param name="handler">The action to execute when the method is called.</param>
    /// <returns>An <see cref="IDisposable"/> that can be disposed to unregister the handler.</returns>
    IDisposable On<T>(string methodName, Action<T> handler);
}