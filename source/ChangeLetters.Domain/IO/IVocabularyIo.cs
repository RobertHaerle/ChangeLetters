using ChangeLetters.Shared;

namespace ChangeLetters.Domain.IO;

/// <summary>Interface IVocabularyIo.</summary>
public interface IVocabularyIo
{
    /// <summary>Loads the vocabulary.</summary>
    /// <returns>See description.</returns>
    Dictionary<string, VocabularyEntry> LoadVocabulary();

    /// <summary>Saves the vocabulary.</summary>
    /// <param name="vocabulary">The vocabulary.</param>
    /// <returns>See description.</returns>
    void SaveVocabulary(Dictionary<string, VocabularyEntry> vocabulary);

    /// <summary>Gets the unknown entries.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    /// <returns>See description.</returns>
    Dictionary<string, VocabularyEntry> GetUnknownEntries(IList<string> unknownWords);

    /// <summary>Saves the unknown entries.</summary>
    /// <param name="unknownEntries">The unknown entries.</param>
    /// <returns>See description.</returns>
    void SaveUnknownEntries(Dictionary<string, VocabularyEntry> unknownEntries);
}