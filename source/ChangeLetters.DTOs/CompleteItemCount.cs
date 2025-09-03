namespace ChangeLetters.DTOs;

/// <summary> 
/// Class CompleteItemCount. 
/// </summary>
public class CompleteItemCount
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteItemCount"/> class.
    /// </summary>
    public CompleteItemCount()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompleteItemCount"/> class.
    /// </summary>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="total">The total.</param>
    public CompleteItemCount(FileItemType fileItemType, int total)
    {
        FileItemType = fileItemType;
        Total = total;
    }
    /// <summary>Gets or sets the type of the file item.</summary>
    public FileItemType FileItemType { get; set; }

    /// <summary>Gets or sets the count.</summary>
    public int Total { get; set; }
}