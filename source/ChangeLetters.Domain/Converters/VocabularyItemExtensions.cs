using ChangeLetters.Domain.Models;
using ChangeLetters.Shared;

namespace ChangeLetters.Domain.Converters;

/// <summary> 
/// Class VocabularyItemExtensions. 
/// </summary>
public static class VocabularyItemExtensions
{
    /// <summary>
    /// Determines whether the <see cref="VocabularyItem"/> is considered corrected.
    /// </summary>
    /// <param name="item">The vocabulary item to check.</param>
    /// <returns><c>true</c> if the item is corrected; otherwise, <c>false</c>.</returns>
    public static bool IsCorrected(this VocabularyItem item)
        => !string.IsNullOrEmpty(item.CorrectedWord) &&
           !item.CorrectedWord.Contains('?');

    /// <summary>
    /// Converts a <see cref="VocabularyItem"/> to a <see cref="VocabularyEntry"/> DTO.
    /// </summary>
    /// <param name="item">The vocabulary item to convert.</param>
    /// <returns>A <see cref="VocabularyEntry"/> DTO.</returns>
    public static VocabularyEntry ToDto(this VocabularyItem item)
        => new()
        {
            UnknownWord = item.UnknownWord,
            CorrectedWord = item.CorrectedWord
        };

    /// <summary>
    /// Converts a <see cref="VocabularyEntry"/> DTO to a <see cref="VocabularyItem"/> model.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A <see cref="VocabularyItem"/> model.</returns>
    public static VocabularyItem ToModel(this VocabularyEntry dto)
        => new()
        {
            UnknownWord = dto.UnknownWord,
            CorrectedWord = dto.CorrectedWord
        };
}