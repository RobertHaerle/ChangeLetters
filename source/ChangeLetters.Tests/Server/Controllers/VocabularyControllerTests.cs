using ChangeLetters.Shared;
using ChangeLetters.Controllers;
using ChangeLetters.Database.Repositories;
using ChangeLetters.Domain.Handlers;
using ChangeLetters.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChangeLetters.Tests.Server.Controllers;

[TestFixture]
public class VocabularyControllerTests
{
    private VocabularyController _sut = null!;
    private IVocabularyHandler _handler = null!;
    private IVocabularyRepository _repository = null!;
    private ILogger<VocabularyController> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = Substitute.For<IVocabularyHandler>();
        _repository = Substitute.For<IVocabularyRepository>();
        _logger = Substitute.For<ILogger<VocabularyController>>();
        _sut = new VocabularyController(_handler, _repository, _logger);
    }

    [TearDown]
    public void TearDown()
        => _sut.Dispose();

    [Test]
    public async Task RebuildAllItemsAsync_CallsRepositoryAndReturnsOk()
    {
        var entries = new List<VocabularyEntry> { new (){UnknownWord = "M?hre", CorrectedWord = "Möhre"} };
        await _sut.RebuildAllItemsAsync(entries);
        await _repository.Received().RecreateAllItemsAsync(Arg.Any<VocabularyItem[]>());
    }

    [Test]
    public async Task UpsertEntriesAsync_CallsRepositoryAndReturnsOk()
    {
        var entries = new List<VocabularyEntry> { new() { UnknownWord = "M?hre", CorrectedWord = "Möhre" } };
        await _sut.UpsertEntriesAsync(entries);
        await _repository.Received().UpsertEntriesAsync(Arg.Any<VocabularyItem[]>());
    }

    [Test]
    public async Task GetAllItemsAsync_ReturnsOkWithDtos()
    {
        var  models = new[] { new VocabularyItem() { UnknownWord = "M?hre", CorrectedWord = "Möhre" } };
        _repository.GetAllItemsAsync(Arg.Any<CancellationToken>()).Returns(models);
        var result = await _sut.GetAllItemsAsync();
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBeAssignableTo<VocabularyEntry[]>();
    }

    [Test]
    public async Task GetRequiredWords_ReturnsOkWithEntries()
    {
        var entries = new List<VocabularyEntry> { new() { UnknownWord = "foo" } };
        _handler.GetRequiredVocabularyAsync(Arg.Any<string[]>(), Arg.Any<CancellationToken>()).Returns(entries);
        var result = await _sut.GetRequiredWords(["foo"]);
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(entries);
    }

    [Test]
    public async Task GetRequiredWordsMassData_ReturnsOkWithEntries()
    {
        var entries = new List<VocabularyEntry> { new () { UnknownWord = "foo" } };
        _handler.GetRequiredVocabularyAsync(Arg.Any<string[]>(), Arg.Any<CancellationToken>()).Returns(entries);
        var result = await _sut.GetRequiredWordsMassData(["foo"]);
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(entries);
    }
}
