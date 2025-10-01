using ChangeLetters.Shared;

namespace ChangeLetters.SignalR;

/// <summary>Interface ISignalRHubRename.</summary>
public interface ISignalRHubRename
{
    /// <summary>Informs the subscriber about the number of the currently processed item.</summary>
    /// <param name="currentItemCount">The current item count.</param>
    /// <returns>See description.</returns>
    [HubMethodName(SignalRPath.Rename.CurrentItemCount)]
    Task CurrentItemNumberChanged(CurrentItemCount currentItemCount);

    /// <summary>Informs the subscriber about the determined amount of file items.</summary>
    /// <param name="completeItemCount">The complete item count.</param>
    [HubMethodName(SignalRPath.Rename.CompleteItemCount)]
    Task ItemCountDetermined(CompleteItemCount completeItemCount);
}