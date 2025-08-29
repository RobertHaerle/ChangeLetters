using ChangeLetters.DTOs;
using ChangeLetters.Model;

namespace ChangeLetters.IO;

/// <summary> 
/// Class VocabularyIo.
/// Implements <see cref="IVocabularyIo" />
/// </summary>
[Export(typeof(IVocabularyIo))]
public class VocabularyIo : IVocabularyIo
{
    private readonly IJsonIo _jsonIo;
    private readonly object _lock = new();
    private readonly ILogger<VocabularyIo> _log;

    public VocabularyIo(IJsonIo jsonIo, ILogger<VocabularyIo> log)
    {
        _log = log;
        _jsonIo = jsonIo;
        _log.LogInformation("instantiated {type}", GetType().Name);
    }

    public Dictionary<string, VocabularyEntry> LoadVocabulary()
    {
        lock (_lock)
        {
            var vocabularyEntries = _jsonIo.Load<List<VocabularyEntry>>("Vocabulary.json");
            return vocabularyEntries.ToDictionary(
                entry => entry.UnknownWord,
                entry => entry,
                StringComparer.OrdinalIgnoreCase);
        }
    }

    public void SaveVocabulary(Dictionary<string, VocabularyEntry> vocabulary)
    {
        lock (_lock)
        {
            var vocabularyEntries = vocabulary.Values.ToList();
            _jsonIo.Save("Vocabulary.json", vocabularyEntries);
        }
    }

    public Dictionary<string, VocabularyEntry> GetUnknownEntries(IList<string> unknownWords)
    {
        var vocabulary = LoadVocabulary();
        var unknownEntries = unknownWords
            .Where(x => !vocabulary.ContainsKey(x))
            .ToDictionary(
                word => word,
                word => new VocabularyEntry { UnknownWord = word, CorrectedWord = word },
                StringComparer.OrdinalIgnoreCase);
        return unknownEntries;
    }

    public void SaveUnknownEntries(Dictionary<string, VocabularyEntry> unknownEntries)
    {
        var vocabulary = LoadVocabulary();
        foreach (var entry in unknownEntries)
            vocabulary[entry.Key] = entry.Value;
        SaveVocabulary(vocabulary);
    }
}
