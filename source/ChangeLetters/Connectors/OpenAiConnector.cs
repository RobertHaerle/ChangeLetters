using OpenAI.Chat;
using System.ClientModel;
using System.Diagnostics;
using ChangeLetters.Extensions;
using ChangeLetters.Configurations;
using ChangeLetters.Wrappers;

namespace ChangeLetters.Connectors;

/// <summary> 
/// Class OpenAiConnector.
/// Implements <see cref="IOpenAiConnector" />
/// </summary>
[Export(typeof(IOpenAiConnector))]
public class OpenAiConnector(
    IChatClient _chatClient,
    OpenAiSettings _settings,
    ILogger<OpenAiConnector> _log) : IOpenAiConnector
{
    /// <inheritdoc />
    public async Task<string?> GetUnknownWordSuggestionAsync(string unknownWord, CancellationToken token)
    {
        _log.LogTrace("Getting suggestion for unknown word: {word}", unknownWord);
        string question = $"Suggest a similar word for the unknown, most possibly German word '{unknownWord}' by changing the question marks. It should be a German special character. Respond with only the suggested word.";
        var messages = new[] { new UserChatMessage(question) };
        var options = GetChatOptions();

        ClientResult<ChatCompletion>? response;
        var stopwatch = Stopwatch.StartNew();
        try
        {
            response = await _chatClient.CompleteChatAsync(messages, options, token);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _log.LogError(ex, $" openAI request failed after {stopwatch.Elapsed.TotalSeconds:F1} seconds.");
            return unknownWord;
        }
        stopwatch.Stop();
        return ExtractSuggestion(response, unknownWord, stopwatch);
    }

    /// <inheritdoc />
    public async Task<(string UnknownWord, string Suggestion)[]> GetUnknownWordSuggestionsAsync(IList<string> unknownWords, CancellationToken token)
    {
        var tasks = unknownWords.Select(async word =>
        {
            var suggestion = await GetUnknownWordSuggestionAsync(word, token);
            return (word, suggestion ?? word);
        });
        return await Task.WhenAll(tasks);
    }

    private string ExtractSuggestion(ClientResult<ChatCompletion>? response, string unknownWord, Stopwatch stopwatch)
    {
        if (response?.Value.Content?.Any() ?? false)
        {
            var suggestion = response.Value.Content[0].Text;
            if (!suggestion.IsNullOrEmpty())
            {
                _log.LogTrace("Suggestion received for unknown word {word}: {suggestedWord}. Required time: {seconds} seconds.", unknownWord, suggestion, stopwatch.Elapsed.TotalSeconds);
                return suggestion;
            }
        }

        _log.LogInformation("No suggestion received for unknown word {word}. Required time: {seconds} seconds.", unknownWord, stopwatch.Elapsed.TotalSeconds);
        return unknownWord;
    }

    private ChatCompletionOptions GetChatOptions()
    {
        var options = new ChatCompletionOptions
        {
            TopP = (float)_settings.TopP,
            MaxOutputTokenCount = _settings.MaxTokens,
            PresencePenalty = (float)_settings.PresencePenalty,
            FrequencyPenalty = (float)_settings.FrequencyPenalty,
        };
        return options;
    }
}