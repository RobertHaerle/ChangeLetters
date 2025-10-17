using ChangeLetters.Models.Models;
using ChangeLetters.Domain.Pocos;
using ChangeLetters.Domain.Configurations;
using ChangeLetters.Database.Repositories;
using ChangeLetters.Domain.AiAccess;

namespace ChangeLetters.Domain.Handlers;

/// <summary> 
/// Class VocabularyHandler.
/// Implements <see cref="IVocabularyHandler" />
/// </summary>
[Export(typeof(IVocabularyHandler))]
internal class VocabularyHandler(
    OpenAiSettings _openAiSettings,
    IAiConnector aiConnector,
    IVocabularyRepository _repository) : IVocabularyHandler
{
    /// <inheritdoc />
    public Task UpsertEntriesAsync(IList<VocabularyItem> items, CancellationToken token)
        => _repository.UpsertEntriesAsync(items, token);

    /// <inheritdoc />
    public Task RecreateAllItemsAsync(IList<VocabularyItem> items, CancellationToken token)
        => _repository.RecreateAllItemsAsync(items, token);

    /// <inheritdoc />
    public Task<VocabularyItem[]> GetAllItemsAsync(CancellationToken token)
        => _repository.GetAllItemsAsync(token);

    /// <inheritdoc />
    public async Task<List<RequiredVocabulary>> GetRequiredVocabularyAsync(IList<string> unknownWords, CancellationToken token)
    {
        try
        {
            var items = await _repository.GetItemsAsync(unknownWords, token).ConfigureAwait(false);
            var entries = items.Select(i => i.ToRequiredVocabulary()).ToList();
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

    private async Task TryResolveWithOpenAiAsync(List<RequiredVocabulary> entries)
    {
        var unknownVocabulary = entries.Where(e => e.CorrectedWord.Contains('?')).ToArray();
        if (unknownVocabulary.Length == 0)
            return;
        var unresolvedWords = unknownVocabulary.Select(e => e.UnknownWord).ToArray();
        var suggestions = await aiConnector.GetUnknownWordSuggestionsAsync(unresolvedWords, CancellationToken.None);
        AddToEntries(suggestions, entries);
    }

    private void AddToEntries((string UnknownWord, string Suggestion)[] suggestions, List<RequiredVocabulary> items)
    {
        foreach (var suggestion in suggestions
                     .Where(s=> !s.Suggestion.Contains('?')))
        {
            var entry = items.FirstOrDefault(e => e.UnknownWord == suggestion.UnknownWord);
            if (entry != null && entry.CorrectedWord.Contains('?'))
            {
                entry.CorrectedWord = suggestion.Suggestion;
                entry.AiResolved = true;
            }
        }
    }

    private void FillUpUnknownWords(List<RequiredVocabulary> entries, IList<string> unknownWords)
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