using ChangeLetters.Domain.Models;
using ChangeLetters.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ChangeLetters.Repositories;

/// <summary> 
/// Class VocabularyRepository.
/// Implements <see cref="IVocabularyRepository" />
/// </summary>
[Export(typeof(IVocabularyRepository))]
public class VocabularyRepository(
    Func<DatabaseContext> _getContext,
    ILogger<VocabularyRepository> _log) : IVocabularyRepository
{
    /// <inheritdoc />
    public async Task UpsertEntriesAsync(IList<VocabularyItem> items)
    {
        var unknownWords = items.Select(x => x.UnknownWord);
        await using var context = _getContext();
        var dbItems = await context.VocabularyItems
            .Where(x => unknownWords.Contains(x.UnknownWord))
            .ToArrayAsync()
            .ConfigureAwait(false);
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        UpsertEntries(context, items, dbItems);
        context.ChangeTracker.DetectChanges();
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RecreateAllItemsAsync(IList<VocabularyItem> items)
    {
        await using var context = _getContext();
        await context.Database.BeginTransactionAsync();
        try
        {
            await context.VocabularyItems.ExecuteDeleteAsync();
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.VocabularyItems.AddRange(items);
            await context.SaveChangesAsync().ConfigureAwait(false);
            await context.Database.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "");
            await context.Database.RollbackTransactionAsync();
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