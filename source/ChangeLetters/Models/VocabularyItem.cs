using ChangeLetters.DTOs;

namespace ChangeLetters.Models;

public class VocabularyItem
{
    public Guid VocabularyItemId { get; set; }

    public required string UnknownWord { get; set; }

    public string CorrectedWord { get; set; } = string.Empty;

    public override string ToString()
        => $"{UnknownWord} -> {CorrectedWord}";
}

public static class VocabularyItemExtensions
{
    public static bool IsCorrected(this VocabularyItem item)
        => !string.IsNullOrEmpty(item.CorrectedWord) &&
           !item.CorrectedWord.Contains('?');

    public static VocabularyEntry ToDto(this VocabularyItem item)
        => new()
        {
            UnknownWord = item.UnknownWord,
            CorrectedWord = item.CorrectedWord
        };

    public static VocabularyItem ToModel(this VocabularyEntry dto)
        => new()
        {
            UnknownWord = dto.UnknownWord,
            CorrectedWord = dto.CorrectedWord
        };
}
