using System.ClientModel;
using OpenAI.Chat;

namespace ChangeLetters.Domain.Wrappers;

/// <summary>
/// Wrapper around ChatClient to enable dependency injection and improve testability.
/// Implements <see cref="IChatClient" />
/// </summary>
[Export(typeof(IChatClient))]
public class ChatClientWrapper(ChatClient _chatClient) : IChatClient
{
    /// <inheritdoc />
    public async Task<ClientResult<ChatCompletion>> CompleteChatAsync(
        IEnumerable<ChatMessage> messages,
        ChatCompletionOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
    }
}