using ChangeLetters.Shared;

namespace ChangeLetters.Handlers;

/// <summary>Interface IVocabularyHandler.</summary>
public interface IVocabularyHandler
{
    /// <summary>Get required vocabulary as an asynchronous operation.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>List of DTOs. unknown words are filled up.</returns>
    Task<List<VocabularyEntry>> GetRequiredVocabularyAsync(IList<string> unknownWords, CancellationToken token);
}