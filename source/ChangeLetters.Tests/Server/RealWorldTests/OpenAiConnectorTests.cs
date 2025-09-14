using ChangeLetters.Models;
using ChangeLetters.Connectors;
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

        _sut = new OpenAiConnector(chatClient, settings, log);
    }

    [Test]
    public async Task GetUnknownWordSuggestion_ShouldReturnSuggestion()
    {
        var result = await _sut.GetUnknownWordSuggestionAsync("G?te", CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();
        result.ShouldBe("Güte");
    }

    [Test]
    public async Task GetUnknownWordSuggestion_ShouldReturnNoSuggestion()
    {
        var result = await _sut.GetUnknownWordSuggestionAsync("Shitb?gertum", CancellationToken.None);
        result.ShouldBe("Shitb?gertum");
    }

    [Test]
    public async Task GetUnknownWordsAsync_ShouldReturnSuggestions()
    {
        var vocabulary = TestExtensions.LoadFromJson<VocabularyItem>(resources.fiftyUnknownWords);
        var unknownWords = vocabulary.Select(v => v.UnknownWord).ToArray();

        var results = await _sut.GetUnknownWordSuggestionsAsync(unknownWords, CancellationToken.None);

        results.Length.ShouldBe(unknownWords.Length);
        results.ShouldNotContain(r => r.UnknownWord.IsNullOrEmpty());
    }
}