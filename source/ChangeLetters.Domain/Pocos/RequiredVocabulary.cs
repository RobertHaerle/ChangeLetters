using ChangeLetters.Models.Models;
using ChangeLetters.Shared;

namespace ChangeLetters.Domain.Pocos;

/// <summary> 
/// Class RequiredVocabulary. 
/// </summary>
public class RequiredVocabulary
{
    /// <summary>Gets or sets the unknown word.</summary>
    public required string UnknownWord { get; set; }

    /// <summary>Gets or sets the corrected word.</summary>
    public string CorrectedWord { get; set; } = string.Empty;

    /// <summary>Gets or sets the information whether this entity contains an AI suggestion.</summary>
    public bool AiResolved { get; set; }
}

/// <summary> 
/// Class RequiredVocabularyExtensions. 
/// </summary>
public static class RequiredVocabularyExtensions
{
    /// <summary>Converts a <see cref="VocabularyItem"/> to a <see cref="RequiredVocabulary"/>.</summary>
    /// <param name="item">The item.</param>
    /// <returns>See description.</returns>
    public static RequiredVocabulary ToRequiredVocabulary(this VocabularyItem item)
        => new()
        {
            UnknownWord = item.UnknownWord,
            CorrectedWord = item.CorrectedWord,
        };

    /// <summary>
    /// Converts a <see cref="RequiredVocabulary"/> to a <see cref="VocabularyEntry"/> DTO.
    /// </summary>
    /// <param name="item">The vocabulary item to convert.</param>
    /// <returns>A <see cref="VocabularyEntry"/> DTO.</returns>
    public static VocabularyEntry ToDto(this RequiredVocabulary item)
        => new()
        {
            AiResolved = item.AiResolved,
            UnknownWord = item.UnknownWord,
            CorrectedWord = item.CorrectedWord
        };
}