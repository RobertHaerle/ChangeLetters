using ChangeLetters.Shared;

namespace ChangeLetters.Domain.IO;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="VocabularyIo"/> class.
    /// </summary>
    /// <param name="jsonIo">The json io.</param>
    /// <param name="log">The log.</param>
    public VocabularyIo(IJsonIo jsonIo, ILogger<VocabularyIo> log)
    {
        _log = log;
        _jsonIo = jsonIo;
        _log.LogInformation("instantiated {type}", GetType().Name);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void SaveVocabulary(Dictionary<string, VocabularyEntry> vocabulary)
    {
        lock (_lock)
        {
            var vocabularyEntries = vocabulary.Values.ToList();
            _jsonIo.Save("Vocabulary.json", vocabularyEntries);
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void SaveUnknownEntries(Dictionary<string, VocabularyEntry> unknownEntries)
    {
        var vocabulary = LoadVocabulary();
        foreach (var entry in unknownEntries)
            vocabulary[entry.Key] = entry.Value;
        SaveVocabulary(vocabulary);
    }
}
