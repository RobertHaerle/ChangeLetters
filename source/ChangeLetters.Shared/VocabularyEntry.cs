namespace ChangeLetters.Shared;

/// <summary> 
/// Class VocabularyEntry. 
/// </summary>
public class VocabularyEntry
{
    /// <summary>Gets or sets the unknown word.</summary>
    public required string UnknownWord { get; set; }

    /// <summary>Gets or sets the corrected word.</summary>
    public string CorrectedWord { get; set; } = string.Empty;

    /// <summary>Determines whether this instance is corrected.</summary>
    public bool IsCorrected ()
        => !string.IsNullOrEmpty(CorrectedWord) 
            && !CorrectedWord.Contains('?')
            && !AiResolved;

    /// <summary>Gets or sets the information whether this entity contains an AI suggestion.</summary>
    public bool AiResolved { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => CorrectedWord;
}
