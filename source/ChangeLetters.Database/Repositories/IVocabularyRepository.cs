namespace ChangeLetters.Database.Repositories;

/// <summary>Interface IVocabularyRepository.</summary>
internal interface IVocabularyRepository
{
    /// <summary>Inserts or update entries as an asynchronous operation.</summary>
    /// <param name="items">The items.</param>
    /// <param name="token"></param>
    Task UpsertEntriesAsync(IList<VocabularyItem> items, CancellationToken token);

    /// <summary>Recreate all items as an asynchronous operation.</summary>
    /// <param name="items">The items.</param>
     Task RecreateAllItemsAsync(IList<VocabularyItem> items, CancellationToken token);

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