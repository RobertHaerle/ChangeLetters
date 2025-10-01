using ChangeLetters.Shared;

namespace ChangeLetters.SignalR;

/// <summary> 
/// SignalR: Informs the subscriber about the progress of a running rename operation.
/// </summary>
[SignalRHub(SignalRPath.Rename.Path)]
public class SignalRHubRename(IConnectionManager<SignalRHubRename> _connectionManager) : Hub<ISignalRHubRename>
{
    /// <inheritdoc />
    [SignalRHidden]
    public override async Task OnConnectedAsync()
    {
        await _connectionManager.AddAsync(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <inheritdoc />
    [SignalRHidden]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _connectionManager.RemoveAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>Informs the subscriber about the determined amount of file items.</summary>
    /// <param name="completeItemCount">Information of item type and the item numbers to be processed.</param>
    /// <returns>See description.</returns>
    [SignalRMethod(SignalRPath.Rename.CompleteItemCount)]
    public Task ItemCountDetermined([SignalRParam] CompleteItemCount completeItemCount)
        => Task.CompletedTask;

    /// <summary>Informs the subscriber about the current item number being processed.</summary>
    /// <param name="currentItemCount">Information of item type and the currently processed item number.</param>
    [SignalRMethod(SignalRPath.Rename.CurrentItemCount)]
    public Task CurrentItemNumberChanged([SignalRParam] CurrentItemCount currentItemCount)
        => Task.CompletedTask;
}
