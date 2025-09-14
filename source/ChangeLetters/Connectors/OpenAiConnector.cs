using OpenAI.Chat;
using ChangeLetters.Configurations;

namespace ChangeLetters.Connectors;

/// <summary> 
/// Class OpenAiConnector.
/// Implements <see cref="IOpenAiConnector" />
/// </summary>
[Export(typeof(IOpenAiConnector))]
public class OpenAiConnector(
    ChatClient chatClient,
    OpenAiSettings settings,
    ILogger<OpenAiConnector> _log) : IOpenAiConnector
{
    /// <inheritdoc />
    public async Task<string?> GetUnknownWordSuggestion(string unknownWord, CancellationToken token)
    {
        _log.LogTrace("Getting suggestion for unknown word: {word}", unknownWord);
        string question = $"Suggest a similar word for the unknown, most possibly German word '{unknownWord}' by changing the question marks. Respond with only the suggested word.";

        var messages = new[] { new UserChatMessage(question) };

        var options = new ChatCompletionOptions
        {
            TopP = (float)settings.TopP,
            MaxOutputTokenCount = settings.MaxTokens,
            //Temperature = (float)settings.Temperature,
            PresencePenalty = (float)settings.PresencePenalty,
            FrequencyPenalty = (float)settings.FrequencyPenalty,
        };

        var response = await chatClient.CompleteChatAsync(messages, options, token);
        if (response?.Value?.Content?.Any() ?? false)
        {
            var suggestion = response.Value.Content[0].Text;
            _log.LogTrace("Suggestion received for unknown word {word}: {suggestedWord}", unknownWord, suggestion);
            return suggestion;
        }
        else
        {
            _log.LogInformation("No suggestion received for unknown word {word}", unknownWord);
            return unknownWord;
        }
    }
}