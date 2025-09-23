using ChangeLetters.Wrappers;
using ChangeLetters.Connectors;
using ChangeLetters.Domain.Models;
using ChangeLetters.Extensions;
using Microsoft.Extensions.Configuration;
using ChangeLetters.Tests.Server.TestHelpers;

namespace ChangeLetters.Tests.Server.RealWorldTests;

[Ignore("real world test")]
public class OpenAiConnectorTests
{
    private OpenAiConnector _sut;

    [SetUp]
    public void Setup()
    {
        var log = Substitute.For<ILogger<OpenAiConnector>>();
        var cfb = new ConfigurationBuilder()
            .AddUserSecrets<OpenAiConnector>()
            .Build();

        var settings = cfb.Deserialize<Configurations.OpenAiSettings>();
        settings.MaxTokens = 1000;
        settings.TopP = 1;
        settings.Model = "gpt-5-nano";

        var chatClient = new OpenAI.Chat.ChatClient(settings.Model, settings.ApiKey);
        var chatClientWrapper = new ChatClientWrapper(chatClient);

        _sut = new OpenAiConnector(chatClientWrapper, settings, log);
    }

    [Test]
    [Ignore("real world test")]
    public async Task GetUnknownWordSuggestion_ShouldReturnSuggestion()
    {
        var result = await _sut.GetUnknownWordSuggestionAsync("G?te", CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();
        result.ShouldBe("Güte");
    }

    [Test]
    [Ignore("real world test")]
    public async Task GetUnknownWordSuggestion_ShouldReturnNoSuggestion()
    {
        var result = await _sut.GetUnknownWordSuggestionAsync("?ngosutt", CancellationToken.None);
        result.ShouldBe("?ngosutt");
    }

    [Test]
    [Ignore("real world test")]
    public async Task GetUnknownWordsAsync_ShouldReturnSuggestions()
    {
        var vocabulary = TestExtensions.LoadFromJson<VocabularyItem>(resources.fiftyUnknownWords);
        var unknownWords = vocabulary.Select(v => v.UnknownWord).ToArray();

        var results = await _sut.GetUnknownWordSuggestionsAsync(unknownWords, CancellationToken.None);

        results.Length.ShouldBe(unknownWords.Length);
        results.ShouldNotContain(r => r.UnknownWord.IsNullOrEmpty());
    }
}