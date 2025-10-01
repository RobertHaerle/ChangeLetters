using ChangeLetters.Models.Models;
using ChangeLetters.Shared;

namespace ChangeLetters.Domain.ParseLogic;

/// <summary>Interface IFileParser.</summary>
public interface IFileParser
{
    /// <summary>Gets the words with unknown letters.</summary>
    /// <param name="items">The items.</param>
    /// <returns>See description.</returns>
    List<string> GetWordsWithUnknownLetters(IList<FileItem> items);


    /// <summary>Tries to replace unknown words in the file name.</summary>
    /// <param name="fileItem">The file item.</param>
    /// <param name="vocabulary">The vocabulary.</param>
    /// <param name="newFileItem">The new file item.</param>
    /// <returns>True or false.</returns>
    bool TryReplaceUnknownWordsInName(FileItem fileItem, Dictionary<string, VocabularyItem> vocabulary,
        out FileItem newFileItem);
}