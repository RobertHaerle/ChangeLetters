using ChangeLetters.DTOs;

namespace ChangeLetters.SignalR;

/// <summary>Interface ISignalRHubRename.</summary>
public interface ISignalRHubRename
{
    /// <summary>Informs the subscriber about the number of the current processed item.</summary>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="currentItem">The current item.</param>
    /// <returns>See description.</returns>
    Task CurrentItemNumberChanged(FileItemType fileItemType, int currentItem);

    /// <summary>Informs the subscriber about the determined amount of file items.</summary>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="itemCount">The item count.</param>
    /// <returns>See description.</returns>
    Task ItemCountDetermined(FileItemType fileItemType, int itemCount);

}