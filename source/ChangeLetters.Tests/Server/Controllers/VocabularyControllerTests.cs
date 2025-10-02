using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ChangeLetters.Shared;
using ChangeLetters.Models.Models;
using ChangeLetters.Domain.Handlers;
using ChangeLetters.Application.Http.Controllers;

namespace ChangeLetters.Tests.Server.Controllers;

[TestFixture]
public class VocabularyControllerTests
{
    private VocabularyController _sut = null!;
    private IVocabularyHandler _handler = null!;
    private ILogger<VocabularyController> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = Substitute.For<IVocabularyHandler>();
        _logger = Substitute.For<ILogger<VocabularyController>>();
        var httpContext = new DefaultHttpContext() { RequestAborted = CancellationToken.None };
        _sut = new VocabularyController(_handler, _logger);
        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [TearDown]
    public void TearDown()
        => _sut.Dispose();

    [Test]
    public async Task RebuildAllItemsAsync_CallsRepositoryAndReturnsOk()
    {
        var entries = new List<VocabularyEntry> { new() { UnknownWord = "M?hre", CorrectedWord = "Möhre" } };
        await _sut.RebuildAllItemsAsync(entries);
        await _handler.Received().RecreateAllItemsAsync(Arg.Any<VocabularyItem[]>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpsertEntriesAsync_CallsRepositoryAndReturnsOk()
    {
        var entries = new List<VocabularyEntry> { new() { UnknownWord = "M?hre", CorrectedWord = "Möhre" } };
        await _sut.UpsertEntriesAsync(entries);
        await _handler.Received().UpsertEntriesAsync(Arg.Any<VocabularyItem[]>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetAllItemsAsync_ReturnsOkWithDtos()
    {
        var models = new[] { new VocabularyItem() { UnknownWord = "M?hre", CorrectedWord = "Möhre" } };
        _handler.GetAllItemsAsync(Arg.Any<CancellationToken>()).Returns(models);
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
        var entries = new List<VocabularyEntry> { new() { UnknownWord = "foo" } };
        _handler.GetRequiredVocabularyAsync(Arg.Any<string[]>(), Arg.Any<CancellationToken>()).Returns(entries);
        var result = await _sut.GetRequiredWordsMassData(["foo"]);
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(entries);
    }
}
