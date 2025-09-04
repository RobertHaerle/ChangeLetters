using ChangeLetters.DTOs;

namespace ChangeLetters.Handlers;

/// <summary>Interface ISignalRRenameHandler.</summary>
public interface ISignalRRenameHandler
{
    /// <summary>Send maximum changes as an asynchronous operation.</summary>
    /// <param name="itemType">Type of the item.</param>
    /// <param name="numberOfItems">The number of items.</param>
    /// <param name="connectionId">The connection identifier.</param>
    Task SendMaxChangesAsync(FileItemType itemType, int numberOfItems, string? connectionId);

    /// <summary>Send current item number as an asynchronous operation.</summary>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="fileItemNumber">The file item number.</param>
    /// <param name="connectionId">The connection identifier.</param>
    Task SendCurrentItemNumberAsync(FileItemType fileItemType, int fileItemNumber, string? connectionId);
}