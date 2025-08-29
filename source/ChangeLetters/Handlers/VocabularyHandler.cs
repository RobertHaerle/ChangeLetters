using ChangeLetters.DTOs;
using ChangeLetters.Models;
using ChangeLetters.Repositories;

namespace ChangeLetters.Handlers;

[Export(typeof(IVocabularyHandler))]
public class VocabularyHandler(
    IVocabularyRepository _repository) : IVocabularyHandler
{
    /// inheritdoc />
    public async Task<List<VocabularyEntry>> GetRequiredVocabularyAsync(IList<string> unknownWords, CancellationToken token)
    {
        try
        {
            var items = await _repository.GetItemsAsync(unknownWords, token).ConfigureAwait(false);
            var entries = items.Select(i => i.ToDto()).ToList();
            FillUpUnknownWords(entries, unknownWords);
            return entries;

        }
        catch (TaskCanceledException)
        {
        }

        return [];
    }

    private void FillUpUnknownWords(List<VocabularyEntry> entries, IList<string> unknownWords)
    {
        var knownWords = new HashSet<string>(entries.Select(e => e.UnknownWord));
        foreach (var unresolvedWord in unknownWords)
        {
            if (!knownWords.Contains(unresolvedWord))
                entries.Add(new()
                {
                    UnknownWord = unresolvedWord,
                    CorrectedWord = unresolvedWord
                });
        }
    }
}