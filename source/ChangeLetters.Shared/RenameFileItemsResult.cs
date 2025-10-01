namespace ChangeLetters.Shared;

/// <summary> 
/// Class RenameFileItemsResult. 
/// </summary>
public class RenameFileItemsResult
{
    /// <summary>Gets or sets the succeeded value of the operation.</summary>
    public required bool Succeeded { get; set; }

    /// <summary>Gets or sets the failed file. This is only set if <see cref="Succeeded"/> is false.</summary>
    public string? FailedFile { get; set; }
}
