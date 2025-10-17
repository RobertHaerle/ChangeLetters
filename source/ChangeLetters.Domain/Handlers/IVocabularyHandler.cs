using ChangeLetters.Domain.Pocos;
using ChangeLetters.Shared;
using ChangeLetters.Models.Models;

namespace ChangeLetters.Domain.Handlers;

/// <summary>Interface IVocabularyHandler.</summary>
public interface IVocabularyHandler
{
    /// <summary>Get required vocabulary as an asynchronous operation.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>List of DTOs. unknown words are filled up.</returns>
    Task<List<RequiredVocabulary>> GetRequiredVocabularyAsync(IList<string> unknownWords, CancellationToken token);

    /// <summary>Recreate all items as an asynchronous operation.</summary>
    /// <param name="items">The items.</param>
    /// <param name="token"></param>
    Task RecreateAllItemsAsync(IList<VocabularyItem> items, CancellationToken token);

    /// <summary>Inserts or update entries as an asynchronous operation.</summary>
    /// <param name="items">The items.</param>
    /// <param name="token"></param>
    Task UpsertEntriesAsync(IList<VocabularyItem> items, CancellationToken token);

    Task<VocabularyItem[]> GetAllItemsAsync(CancellationToken token);
}