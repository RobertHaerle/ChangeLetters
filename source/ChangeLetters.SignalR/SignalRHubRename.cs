using ChangeLetters.DTOs;

namespace ChangeLetters.SignalR;

/// <summary> 
/// Class SignalRHubRename.
/// Inherits from <see cref="Hub{ISignalRHubRename}" />
/// </summary>
[SignalRHub]
public class SignalRHubRename(IConnectionManager<SignalRHubRename> _connectionManager) : Hub<ISignalRHubRename>
{
    /// inheritdoc />
    public override async Task OnConnectedAsync()
    {
        await _connectionManager.AddAsync(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// inheritdoc />
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _connectionManager.RemoveAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>Informs the subscriber about the determined amount of file items.</summary>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="itemCount">The item count.</param>
    /// <returns>See description.</returns>
    public Task ItemCountDetermined(FileItemType fileItemType, int itemCount)
        => Task.CompletedTask;

}
