using ChangeLetters.Shared;

namespace ChangeLetters.Domain.Ftp;

internal class FtpItem
{
    /// <summary>Gets or sets the name.</summary>
    public required string Name { get; set; }

    /// <summary>Gets or sets the full name.</summary>
    public required string FullName { get; set; }

    /// <summary>Gets or sets the is folder.</summary>
    public bool IsFolder { get; set; }

    /// <summary>Gets or sets the folder status.</summary>
    public FtpFolderStatus FolderStatus { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => FullName;
}

/// <summary>Enum FolderStatus</summary>
public enum FtpFolderStatus
{
    /// <summary> The undefined state. </summary>
    Undefined,
    /// <summary> The ok state: No question marks in the folder name. </summary>
    Ok,
    /// <summary> The has question marks state: There are question marks in the folder name. </summary>
    HasQuestionMarks
}
