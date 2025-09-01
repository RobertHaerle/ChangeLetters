namespace ChangeLetters.SignalR;

/// <summary>Interface ISignalRHubRename.</summary>
public interface ISignalRHubRename
{
    Task CurrentFileCount(int currentItem);
    Task FileCountDetermined(int numberOfItems);

}