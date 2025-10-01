using ChangeLetters.Extensions;
using ChangeLetters.Models.Models;
using ChangeLetters.Shared;

namespace ChangeLetters.ParseLogic;

/// <summary> 
/// Class FileParser.
/// Implements <see cref="IFileParser" />
/// </summary>
[Export(typeof(IFileParser))]
public class FileParser : IFileParser
{
    /// <inheritdoc />
    public List<string> GetWordsWithUnknownLetters(IList<FileItem> items)
    {
        var resultSet = new List<string>();
        foreach (var item in items)
        {
            if (item.Name.Contains("?"))
                resultSet.AddRange(GetWordsWithQuestionMarks(item.Name));
        }

        resultSet = resultSet.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        return resultSet;
    }

    /// <inheritdoc />
    public bool TryReplaceUnknownWordsInName(FileItem fileItem, Dictionary<string, VocabularyItem> vocabulary, 
        out FileItem newFileItem)
    {
        var words = GetWordsWithQuestionMarks(fileItem.Name);
        var newFileFullName = fileItem.FullName;
        var newFileName = fileItem.Name;
        foreach (var word in words)
        {
            if (vocabulary.TryGetValue(word, out var entry))
                newFileName = newFileName.Replace(word, entry.CorrectedWord, StringComparison.OrdinalIgnoreCase);
        }

        var path = Path.GetDirectoryName(fileItem.FullName);
        if (path != null)
            newFileFullName = Path.Combine(path, newFileName).ToLinuxPath();

        newFileItem = new FileItem { FullName = newFileFullName!, Name = newFileName };
        return !newFileName.Contains('?');
    }


    private IEnumerable<string> GetWordsWithQuestionMarks(string fileName)
    {
        return fileName.Split(' ', '.', '-')
            .Where(x => x.Contains('?'))
            .Select(x => x.Trim('(', ')', ',', '\'', '_', '"'));
    }
}
