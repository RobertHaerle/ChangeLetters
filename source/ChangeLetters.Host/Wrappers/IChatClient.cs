using OpenAI.Chat;
using System.ClientModel;

namespace ChangeLetters.Wrappers;

/// <summary>
/// Interface for ChatClient wrapper to enable dependency injection and testing.
/// </summary>
public interface IChatClient
{
    /// <summary>
    /// Completes a chat conversation asynchronously.
    /// </summary>
    /// <param name="messages">The chat messages.</param>
    /// <param name="options">The chat completion options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the chat completion response.</returns>
    Task<ClientResult<ChatCompletion>> CompleteChatAsync(
        IEnumerable<ChatMessage> messages,
        ChatCompletionOptions? options = null,
        CancellationToken cancellationToken = default);
}