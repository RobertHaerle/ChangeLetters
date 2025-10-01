using ChangeLetters.Domain.Models;

namespace ChangeLetters.Repositories;

/// <summary>Interface IVocabularyRepository.</summary>
public interface IVocabularyRepository
{
    /// <summary>Inserts or update entries as an asynchronous operation.</summary>
    /// <param name="items">The items.</param>
    /// <returns>See description.</returns>
     Task UpsertEntriesAsync(IList<VocabularyItem> items);

    /// <summary>Recreate all items as an asynchronous operation.</summary>
    /// <param name="items">The items.</param>
    /// <returns>See description.</returns>
     Task RecreateAllItemsAsync(IList<VocabularyItem> items);

    /// <summary>Get all items as an asynchronous operation.</summary>
    /// <param name="token"></param>
    /// <returns>See description.</returns>
    Task<VocabularyItem[]> GetAllItemsAsync(CancellationToken token);

    /// <summary>Get items as an asynchronous operation.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    /// <param name="token"></param>
    /// <returns>See description.</returns>
    Task<List<VocabularyItem>> GetItemsAsync(IList<string> unknownWords, CancellationToken token);
}