namespace ChangeLetters.Connectors;

/// <summary>Interface IOpenAiConnector.</summary>
public interface IOpenAiConnector
{
    /// <summary>Gets the unknown word suggestion.</summary>
    /// <param name="unknownWord">The unknown word.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>the possible AI generated resolution.</returns>
    Task<string?> GetUnknownWordSuggestionAsync(string unknownWord, CancellationToken token);

    /// <summary>Get suggestions for a bunch of unknown words as an asynchronous operation.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<(string UnknownWord, string Suggestion)[]> GetUnknownWordSuggestionsAsync(IList<string> unknownWords, CancellationToken token);
}