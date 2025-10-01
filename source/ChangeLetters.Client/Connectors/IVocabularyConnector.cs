using ChangeLetters.Shared;

namespace ChangeLetters.Client.Connectors;

public interface IVocabularyConnector
{
    Task UpsertEntriesAsync(ICollection<VocabularyEntry> entries);
    Task RebuildAllItemsAsync(ICollection<VocabularyEntry> entries);
    Task<Dictionary<string, VocabularyEntry>> GetAllItemsAsync();
}