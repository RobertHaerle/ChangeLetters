using ChangeLetters.Shared;

namespace ChangeLetters.Client.Connectors;

/// <summary>Interface ISignalRRenameConnector.</summary>
public interface ISignalRRenameConnector : IAsyncDisposable
{
    /// <summary> Occurs when th eSignalR connection identifier changed.</summary>
    event Action<string> ConnectionIdChanged;

    /// <summary> Occurs when current item count was changed. </summary>
    event Action<CurrentItemCount>? CurrentItemCountChanged;

    /// <summary>Occurs when complete item count was changed. </summary>
    event Action<CompleteItemCount>? CompleteItemCountChanged;

    /// <summary>Connect as an asynchronous operation.</summary>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The connection ID.</returns>
    Task<string> ConnectAsync(CancellationToken token);

    /// <summary>Close the SignalR connection.</summary>
    Task CloseAsync();
}