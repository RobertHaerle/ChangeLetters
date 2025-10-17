namespace ChangeLetters.Database.Repositories;

/// <summary> 
/// Class VocabularyRepository.
/// Implements <see cref="IVocabularyRepository" />
/// </summary>
[Export(typeof(IVocabularyRepository))]
internal class VocabularyRepository(
    Func<DatabaseContext> _getContext,
    ILogger<VocabularyRepository> _log) : IVocabularyRepository
{
    /// <inheritdoc />
    public async Task UpsertEntriesAsync(IList<VocabularyItem> items, CancellationToken token)
    {
        var unknownWords = items.Select(x => x.UnknownWord);
        await using var context = _getContext();
        try
        {
            var dbItems = await context.VocabularyItems
                .Where(x => unknownWords.Contains(x.UnknownWord))
                .ToArrayAsync(token)
                .ConfigureAwait(false);
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            UpsertEntries(context, items, dbItems);
            context.ChangeTracker.DetectChanges();
            await context.SaveChangesAsync(token).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
        }
    }

    /// <inheritdoc />
    public async Task RecreateAllItemsAsync(IList<VocabularyItem> items, CancellationToken token)
    {
        await using var context = _getContext();
        try
        {
            await context.Database.BeginTransactionAsync(token);
            await context.VocabularyItems.ExecuteDeleteAsync(token);
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.VocabularyItems.AddRange(items);
            await context.SaveChangesAsync(token).ConfigureAwait(false);
            await context.Database.CommitTransactionAsync(token);
        }
        catch (TaskCanceledException)
        {
            await context.Database.RollbackTransactionAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "");
            await context.Database.RollbackTransactionAsync(token);
        }
    }

    /// <inheritdoc />
    /// <param name="token"></param>
    public async Task<VocabularyItem[]> GetAllItemsAsync(CancellationToken token)
    {
        await using var context = _getContext();
        try
        {
            var resultSet = await context.VocabularyItems
                .AsNoTracking()
                .ToArrayAsync(token)
                .ConfigureAwait(false);

            return resultSet;
        }
        catch (TaskCanceledException)
        {
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<List<VocabularyItem>> GetItemsAsync(IList<string> unknownWords, CancellationToken token)
    {
        await using var context = _getContext();
        try
        {
            var resultSet = await context.VocabularyItems
                .AsNoTracking()
                .Where(x => unknownWords.Contains(x.UnknownWord))
                .ToListAsync(token)
                .ConfigureAwait(false);

            return resultSet;
        }
        catch (TaskCanceledException)
        {
            return [];
        }
    }

    private void UpsertEntries(
        DatabaseContext context,
        IList<VocabularyItem> items,
        VocabularyItem[] dbItems)
    {
        foreach (var item in items)
        {
            var dbItem = dbItems.FirstOrDefault(i => i.UnknownWord == item.UnknownWord);

            if (dbItem != null)
            {
                if (dbItem!.CorrectedWord != item.CorrectedWord)
                    dbItem.CorrectedWord = item.CorrectedWord;
            }
            else
                context.VocabularyItems.Add(item);
        }
    }
}