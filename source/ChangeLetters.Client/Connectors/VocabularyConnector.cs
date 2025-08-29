using ChangeLetters.DTOs;
using System.Net.Http.Json;

namespace ChangeLetters.Client.Connectors;

public class VocabularyConnector(HttpClient _httpClient) : IVocabularyConnector
{
    public async Task UpsertEntriesAsync(ICollection<VocabularyEntry> entries)
    {
        var response = await _httpClient.PutAsJsonAsync("api/Vocabulary/Upsert", entries)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task RebuildAllItemsAsync(ICollection<VocabularyEntry> entries)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Vocabulary/RebuildAll", entries)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Dictionary<string, VocabularyEntry>> GetAllItemsAsync()
    {
        var response = await _httpClient.GetAsync("api/Vocabulary")
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var dtos = await response.Content
            .ReadFromJsonAsync<List<VocabularyEntry>>()
            .ConfigureAwait(false);

        return dtos!.ToDictionary(d => d.UnknownWord);
    }
}