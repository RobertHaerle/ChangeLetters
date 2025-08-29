namespace ChangeLetters.DTOs;

/// <summary> 
/// Class FileItem. 
/// </summary>
public class FileItem
{
    /// <summary>Gets or sets the name.</summary>
    public required string Name { get; set; }

    /// <summary>Gets or sets the full name.</summary>
    public required string FullName { get; set; }

    /// <summary>Gets or sets the is folder.</summary>
    public bool IsFolder { get; set; }

    /// <summary>Gets or sets the folder status.</summary>
    public FolderStatus FolderStatus { get; set; }

    /// inheritdoc />
    public override string ToString()
        => FullName;
}

/// <summary>Enum FolderStatus</summary>
public enum FolderStatus
{
    Undefined, 
    Ok,
    HasQuestionMarks
}