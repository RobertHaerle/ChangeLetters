namespace ChangeLetters.Domain.Models;

/// <summary> 
/// Class VocabularyItem. 
/// </summary>
public class VocabularyItem
{
    /// <summary>Gets or sets the vocabulary item identifier.</summary>
    public Guid VocabularyItemId { get; set; }

    /// <summary>Gets or sets the unknown word.</summary>
    public required string UnknownWord { get; set; }

    /// <summary>Gets or sets the corrected word.</summary>
    public string CorrectedWord { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
        => $"{UnknownWord} -> {CorrectedWord}";
}

