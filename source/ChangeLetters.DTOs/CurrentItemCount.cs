namespace ChangeLetters.DTOs;

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
    /// <param name="count">The count.</param>
    public CurrentItemCount(FileItemType fileItemType, int count)
    {
        FileItemType = fileItemType;
        Count = count;
    }

    /// <summary>Gets or sets the type of the file item.</summary>
    public FileItemType FileItemType { get; set; }

    /// <summary>Gets or sets the count.</summary>
    public int Count { get; set; }
}
