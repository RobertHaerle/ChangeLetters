using ChangeLetters.Connectors;
using ChangeLetters.Extensions;
using Microsoft.Extensions.Configuration;

namespace ChangeLetters.Tests.Server.RealWorldTests;


public class OpenAiConnectorTests
{
    private OpenAiConnector _sut;

    [SetUp]
    [Ignore("real world test")]
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
        var result = await _sut.GetUnknownWordSuggestion("G?te", CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();
        result.ShouldBe("Güte");
    }
}
