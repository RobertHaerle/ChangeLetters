using ChangeLetters.Domain.Models;
using Microsoft.Data.Sqlite;
using ChangeLetters.Repositories;
using Microsoft.EntityFrameworkCore;
using ChangeLetters.Tests.Server.TestHelpers;

namespace ChangeLetters.Tests.Server.Repositories;

public class VocabularyRepositoryTests : IDisposable
{
    private VocabularyRepository _sut;
    private SqliteConnection _connection;
    private IList<VocabularyItem> _vocabulary;

    [SetUp]
    public void SetUp()
    {
        _connection = SqliteHelper.GetSqliteInMemoryConnection();
        _vocabulary = TestExtensions.LoadFromJson<VocabularyItem>(resources.Vocabulary);
        using var context = SqliteHelper.GetInMemoryContext(_connection, true);
        context.VocabularyItems.AddRange(_vocabulary);
        context.SaveChanges();

        _sut = new(() => SqliteHelper.GetInMemoryContext(_connection),
            Substitute.For<ILogger<VocabularyRepository>>());
    }

    public void Dispose()
        => _connection.Dispose();

    [Test]
    public async Task RecreateAllItemsAsync()
    {
        var items = new VocabularyItem[]
        {
            new() { UnknownWord = "K?se", CorrectedWord = "Käse" },
            new() { UnknownWord = "M?hre", CorrectedWord = "Möhre" },
        };

        await _sut.RecreateAllItemsAsync(items);

        await using var context = SqliteHelper.GetInMemoryContext(_connection);
        var allItems = await context.VocabularyItems.ToArrayAsync();
        allItems.Length.ShouldBe(2);
        allItems.ShouldNotContain(x=> x.VocabularyItemId == Guid.Empty);
        allItems.ShouldContain(x => x.UnknownWord == "K?se" && x.CorrectedWord == "Käse");
        allItems.ShouldContain(x => x.UnknownWord == "M?hre" && x.CorrectedWord == "Möhre");
    }

    [Test]
    public async Task GetAllItemsAsync()
    {
        var allItems = await _sut.GetAllItemsAsync(CancellationToken.None);

        allItems.Length.ShouldBe(_vocabulary.Count);
    }

    [Test]
    public async Task GetItemsAsync()
    {
        var requiredWords = new[] { "Ungek?rzt", "Pr?sidenten", "Pr?sident" };

        var items = await _sut.GetItemsAsync(requiredWords, CancellationToken.None);

        items.Count.ShouldBe(2);
        items.ShouldNotContain(x => x.UnknownWord == "Pr?sidenten" );
        items.ShouldContain(x => x.UnknownWord == "Ungek?rzt" && x.CorrectedWord == "Ungekürzt");
        items.ShouldContain(x => x.UnknownWord == "Pr?sident" && x.CorrectedWord == "Präsident");
    }


    [Test]
    public async Task UpsertEntriesAsync()
    {
        var items2Bchanged = new VocabularyItem[]
        {
            new() { UnknownWord = "Ungek?rzt", CorrectedWord = "Ungekürzt" },
            new() { UnknownWord = "Pr?sident", CorrectedWord = "Präsident" },
            new() { UnknownWord = "Pr?sidenten", CorrectedWord = "Präsidenten" },
        };

        await _sut.UpsertEntriesAsync(items2Bchanged);

        await using var context = SqliteHelper.GetInMemoryContext(_connection);
        var allItems = await context.VocabularyItems.ToArrayAsync();
        allItems.Length.ShouldBe(_vocabulary.Count + 1);
        allItems.ShouldContain(x => x.UnknownWord == "Ungek?rzt" && x.CorrectedWord == "Ungekürzt");
        allItems.ShouldContain(x => x.UnknownWord == "Pr?sident" && x.CorrectedWord == "Präsident");
        allItems.ShouldContain(x => x.UnknownWord == "Pr?sidenten" && x.CorrectedWord == "Präsidenten");
    }
}
