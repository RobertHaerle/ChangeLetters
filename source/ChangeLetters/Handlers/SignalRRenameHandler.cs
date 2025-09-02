using ChangeLetters.DTOs;
using ChangeLetters.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace ChangeLetters.Handlers;

/// <summary> 
/// Class SignalRRenameHandler.
/// Implements <see cref="ISignalRRenameHandler" />
/// </summary>
[Export(typeof(ISignalRRenameHandler))]
public class SignalRRenameHandler(
    ILogger<SignalRRenameHandler> _log,
    IConnectionManager<SignalRHubRename> _connectionManager,
    IHubContext<SignalRHubRename, ISignalRHubRename> _hubContext) : ISignalRRenameHandler
{
    /// inheritdoc />
    public async Task SendMaxChangesAsync(FileItemType itemType, int numberOfItems, string? connectionId)
    {
        if (await _connectionManager.ExistsAsync(connectionId))
        {
            _log.LogDebug("Sending max changes {numberOfItems} of type {itemType} to connection {connectionId}", numberOfItems, itemType, connectionId);
            await _hubContext.Clients.Client(connectionId!).ItemCountDetermined(new(itemType, numberOfItems));
        }
        else
        {
            _log.LogWarning("Connection {connectionId} does not exist. Cannot send max changes {numberOfItems} of type {itemType}", connectionId, numberOfItems, itemType);
        }
    }

    /// inheritdoc />
    public async Task SendCurrentItemNumberAsync(FileItemType fileItemType, int fileItemNumber, string? connectionId)
    {
        if (await _connectionManager.ExistsAsync(connectionId))
        {
            _log.LogDebug("Sending current item number {fileItemNumber} of type {fileItemType} to connection {connectionId}", fileItemNumber, fileItemType, connectionId);
            await _hubContext.Clients.Client(connectionId!).CurrentItemNumberChanged(new(fileItemType, fileItemNumber));
        }
        else
        {
            _log.LogWarning("Connection {connectionId} does not exist. Cannot send current item number {fileItemNumber} of type {fileItemType}", connectionId, fileItemNumber, fileItemType);
        }
    }
}