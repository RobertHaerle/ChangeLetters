using OpenAI.Chat;
using ChangeLetters.Extensions;
using ChangeLetters.Configurations;

namespace ChangeLetters.Connectors;

/// <summary> 
/// Class OpenAiConnector.
/// Implements <see cref="IOpenAiConnector" />
/// </summary>
[Export(typeof(IOpenAiConnector))]
public class OpenAiConnector(
    ChatClient _chatClient,
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

        var response = await _chatClient.CompleteChatAsync(messages, options, token);
        if (response?.Value?.Content?.Any() ?? false)
        {
            var suggestion = response.Value.Content[0].Text;
            if (suggestion.IsNullOrEmpty())
                suggestion = unknownWord;
            _log.LogTrace("Suggestion received for unknown word {word}: {suggestedWord}", unknownWord, suggestion);
            return suggestion;
        }
        else
        {
            _log.LogInformation("No suggestion received for unknown word {word}", unknownWord);
            return unknownWord;
        }
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