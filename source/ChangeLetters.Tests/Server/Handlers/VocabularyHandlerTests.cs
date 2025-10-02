using ChangeLetters.Models.Models;
using ChangeLetters.Domain.Handlers;
using ChangeLetters.Domain.Connectors;
using ChangeLetters.Database.Repositories;
using ChangeLetters.Domain.Configurations;

namespace ChangeLetters.Tests.Server.Handlers;

[TestFixture]
public class VocabularyHandlerTests
{
    private VocabularyHandler _sut;
    private OpenAiSettings _openAiSettings;
    private IVocabularyRepository _repository;
    private IOpenAiConnector _openAiConnector;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IVocabularyRepository>();
        _openAiConnector = Substitute.For<IOpenAiConnector>();
        _openAiSettings = new OpenAiSettings { UseOpenAI = false };
        _sut = new VocabularyHandler(_openAiSettings, _openAiConnector, _repository);
    }

    [Test]
    public async Task UpsertEntriesAsync_CallsRepository()
    {
        var items = new List<VocabularyItem>();
        var token = CancellationToken.None;

        await _sut.UpsertEntriesAsync(items, token);

        await _repository.Received(1).UpsertEntriesAsync(items, token);
    }

    [Test]
    public async Task RecreateAllItemsAsync_CallsRepository()
    {
        var items = new List<VocabularyItem>();
        var token = CancellationToken.None;

        await _sut.RecreateAllItemsAsync(items, token);

        await _repository.Received(1).RecreateAllItemsAsync(items, token);
    }

    [Test]
    public async Task GetAllItemsAsync_CallsRepository()
    {
        var token = CancellationToken.None;

        await _sut.GetAllItemsAsync(token);

        await _repository.Received(1).GetAllItemsAsync(token);
    }

    [Test]
    public async Task GetRequiredVocabularyAsync_FillsUpUnknownWords()
    {
        var unknownWords = new List<string> { "foo", "bar" };
        var token = CancellationToken.None;
        _repository.GetItemsAsync(unknownWords, token)
            .Returns(Task.FromResult(new List<VocabularyItem>()));

        var result = await _sut.GetRequiredVocabularyAsync(unknownWords, token);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].UnknownWord, Is.EqualTo("foo"));
        Assert.That(result[1].UnknownWord, Is.EqualTo("bar"));
    }

    [Test]
    public async Task GetRequiredVocabularyAsync_UsesOpenAi_WhenEnabled()
    {
        _openAiSettings.UseOpenAI = true;
        _sut = new VocabularyHandler(_openAiSettings, _openAiConnector, _repository);

        var unknownWords = new List<string> { "foo" };
        var token = CancellationToken.None;
        _repository.GetItemsAsync(unknownWords, token)
            .Returns(Task.FromResult(new List<VocabularyItem>
            {
                new VocabularyItem { UnknownWord = "foo", CorrectedWord = "?" }
            }));

        _openAiConnector.GetUnknownWordSuggestionsAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new[] { ("foo", "bar") }));

        var result = await _sut.GetRequiredVocabularyAsync(unknownWords, token);

        Assert.That(result[0].CorrectedWord, Is.EqualTo("bar"));
        Assert.That(result[0].AiResolved, Is.True);
    }

    [Test]
    public async Task GetRequiredVocabularyAsync_ReturnsEmptyList_OnTaskCanceledException()
    {
        var unknownWords = new List<string> { "foo" };
        var token = CancellationToken.None;
        _repository.GetItemsAsync(unknownWords, token)
            .Returns<Task<List<VocabularyItem>>>(_ => throw new TaskCanceledException());

        var result = await _sut.GetRequiredVocabularyAsync(unknownWords, token);

        Assert.That(result, Is.Empty);
    }
}