namespace ChangeLetters.DTOs;

/// <summary> 
/// Class RenameFileItemsRequest. 
/// </summary>
public class RenameFileItemsRequest
{
    /// <summary>Gets or sets the folder.</summary>
    public required string Folder { get; set; }

    /// <summary>Gets or sets the type of the file item.</summary>
    public required FileItemType FileItemType{ get; set; }
}
