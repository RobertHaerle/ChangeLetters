using ChangeLetters.Database.Repositories;
using ChangeLetters.Domain.Configurations;
using ChangeLetters.Domain.Connectors;
using ChangeLetters.Models.Converters;
using ChangeLetters.Shared;

namespace ChangeLetters.Domain.Handlers;

/// <summary> 
/// Class VocabularyHandler.
/// Implements <see cref="IVocabularyHandler" />
/// </summary>
[Export(typeof(IVocabularyHandler))]
public class VocabularyHandler(
    OpenAiSettings _openAiSettings,
    IOpenAiConnector _openAiConnector,
    IVocabularyRepository _repository) : IVocabularyHandler
{
    /// <inheritdoc />
    public async Task<List<VocabularyEntry>> GetRequiredVocabularyAsync(IList<string> unknownWords, CancellationToken token)
    {
        try
        {
            var items = await _repository.GetItemsAsync(unknownWords, token).ConfigureAwait(false);
            var entries = items.Select(i => i.ToDto()).ToList();
            FillUpUnknownWords(entries, unknownWords);
            if (_openAiSettings.UseOpenAI)
                await TryResolveWithOpenAiAsync(entries);
            return entries;
        }
        catch (TaskCanceledException)
        {
        }

        return [];
    }

    private async Task TryResolveWithOpenAiAsync(List<VocabularyEntry> entries)
    {
        var unknownVocabulary = entries.Where(e => e.CorrectedWord.Contains('?')).ToArray();
        if (unknownVocabulary.Length == 0)
            return;
        var unresolvedWords = unknownVocabulary.Select(e => e.UnknownWord).ToArray();
        var suggestions = await _openAiConnector.GetUnknownWordSuggestionsAsync(unresolvedWords, CancellationToken.None);
        AddToEntries(suggestions, entries);
    }

    private void AddToEntries((string UnknownWord, string Suggestion)[] suggestions, List<VocabularyEntry> entries)
    {
        foreach (var suggestion in suggestions
                     .Where(s=> !s.Suggestion.Contains('?')))
        {
            var entry = entries.FirstOrDefault(e => e.UnknownWord == suggestion.UnknownWord);
            if (entry != null && entry.CorrectedWord.Contains('?'))
            {
                entry.CorrectedWord = suggestion.Suggestion;
                entry.AiResolved = true;
            }
        }
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