namespace ChangeLetters.Shared;

/// <summary> 
/// Class CurrentItemCount. 
/// </summary>
public class CurrentItemCount
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentItemCount"/> class.
    /// </summary>
    public CurrentItemCount()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentItemCount"/> class.
    /// </summary>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="current">The current.</param>
    public CurrentItemCount(FileItemType fileItemType, int current)
    {
        FileItemType = fileItemType;
        Current = current;
    }

    /// <summary>Gets or sets the type of the file item.</summary>
    public FileItemType FileItemType { get; set; }

    /// <summary>Gets or sets the count.</summary>
    public int Current { get; set; }
}
