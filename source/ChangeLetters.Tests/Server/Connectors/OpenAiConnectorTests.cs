using OpenAI.Chat;
using System.ClientModel;
using System.ClientModel.Primitives;
using ChangeLetters.Ai.OpenAi;
using ChangeLetters.Domain.Configurations;
using ChangeLetters.Domain.Wrappers;
using ChangeLetters.Tests.Server.TestHelpers;

namespace ChangeLetters.Tests.Server.Connectors;

[TestFixture]
public class OpenAiConnectorTests
{
    private OpenAiConnector _sut;
    private IChatClient _chatClient;
    private OpenAiSettings _settings ;
    private ILogger<OpenAiConnector> _log;

    [SetUp]
    public void SetUp()
    {
        _chatClient = Substitute.For<IChatClient>();
        _log = Substitute.For<ILogger<OpenAiConnector>>();
        _settings = new OpenAiSettings
        {
            Model = "gpt-5-nano",
            ApiKey = "test-api-key",
            TopP = 1.0F,
            MaxTokens = 100,
            PresencePenalty = 0.0F,
            FrequencyPenalty = 0.0F
        };
        
        _sut = new OpenAiConnector(_chatClient, _settings, _log);
    }

    [Test]
    public async Task GetUnknownWordSuggestionAsync_WithValidResponse_ReturnsSuggestion()
    {
        // Arrange
        const string unknownWord = "M?hre";
        const string expectedSuggestion = "Möhre";
        var cancellationToken = CancellationToken.None;

        var response = CreateResponseWithText(expectedSuggestion);
        _chatClient.CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken)
            .Returns(response);

        // Act
        var result = await _sut.GetUnknownWordSuggestionAsync(unknownWord, cancellationToken);

        // Assert
        result.ShouldBe(expectedSuggestion);
        await _chatClient.Received(1).CompleteChatAsync(
            Arg.Is<IEnumerable<ChatMessage>>(msgs => msgs.Count() == 1),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken);
    }

    [Test]
    public async Task GetUnknownWordSuggestionAsync_WithEmptyResponse_ReturnsOriginalWord()
    {
        // Arrange
        const string unknownWord = "T?st";
        var cancellationToken = CancellationToken.None;

        var response = CreateResponseWithText("");
        _chatClient.CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken)
            .Returns(response);

        // Act
        var result = await _sut.GetUnknownWordSuggestionAsync(unknownWord, cancellationToken);

        // Assert
        result.ShouldBe(unknownWord);
    }


    [Test]
    public async Task GetUnknownWordSuggestionAsync_WithException_ReturnsOriginalWord()
    {
        // Arrange
        const string unknownWord = "T?st";
        var cancellationToken = CancellationToken.None;
        var expectedException = new InvalidOperationException("OpenAI service unavailable");

        _chatClient.CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken)
            .Returns<ClientResult<ChatCompletion>>(x => throw expectedException);

        // Act
        var result = await _sut.GetUnknownWordSuggestionAsync(unknownWord, cancellationToken);

        // Assert
        result.ShouldBe(unknownWord);
    }

    [Test]
    public async Task GetUnknownWordSuggestionsAsync_WithMultipleWords_ReturnsAllSuggestions()
    {
        // Arrange
        var unknownWords = new[] { "M?hre", "G?te", "Sch?n" };
        var expectedSuggestions = new[] { "Möhre", "Güte", "Schön" };
        var cancellationToken = CancellationToken.None;

        var callCount = 0;
        _chatClient.CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken)
            .Returns(x => CreateResponseWithText(expectedSuggestions[callCount++]));

        // Act
        var results = await _sut.GetUnknownWordSuggestionsAsync(unknownWords, cancellationToken);

        // Assert
        results.Length.ShouldBe(unknownWords.Length);
        
        for (int i = 0; i < unknownWords.Length; i++)
        {
            results.ShouldContain(r => r.UnknownWord == unknownWords[i] && r.Suggestion == expectedSuggestions[i]);
        }

        await _chatClient.Received(unknownWords.Length).CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken);
    }

    [Test]
    public async Task GetUnknownWordSuggestionsAsync_WithSomeFailures_ReturnsOriginalWordsForFailures()
    {
        // Arrange
        var unknownWords = new[] { "M?hre", "InvalidWord", "G?te" };
        var cancellationToken = CancellationToken.None;

        var callCount = 0;
        _chatClient.CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken)
            .Returns(x =>
            {
                callCount++;
                return callCount switch
                {
                    1 => CreateResponseWithText("Möhre"),
                    2 => throw new InvalidOperationException("API Error"),
                    3 => CreateResponseWithText("Güte"),
                    _ => throw new InvalidOperationException("Unexpected call")
                };
            });

        // Act
        var results = await _sut.GetUnknownWordSuggestionsAsync(unknownWords, cancellationToken);

        // Assert
        results.Length.ShouldBe(3);
        results.ShouldContain(r => r.UnknownWord == "M?hre" && r.Suggestion == "Möhre");
        results.ShouldContain(r => r.UnknownWord == "InvalidWord" && r.Suggestion == "InvalidWord"); // Should return original word on failure
        results.ShouldContain(r => r.UnknownWord == "G?te" && r.Suggestion == "Güte");
    }

    [Test]
    public async Task GetUnknownWordSuggestionAsync_CallsWithCorrectChatOptions()
    {
        // Arrange
        const string unknownWord = "T?st";
        var cancellationToken = CancellationToken.None;
        ChatCompletionOptions options = null;
        var response = CreateResponseWithText("Test");
        _chatClient.CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken)
            .Returns(response);

        _chatClient.When(x=> x.CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken))
            .Do(ci => options = ci.ArgAt<ChatCompletionOptions>(1));

        // Act
        await _sut.GetUnknownWordSuggestionAsync(unknownWord, cancellationToken);

        // Assert
        await _chatClient.Received(1).CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            cancellationToken);

        options.TopP!.Value.ShouldBe(_settings.TopP);
        options.MaxOutputTokenCount.ShouldBe(_settings.MaxTokens);
        options.PresencePenalty.ShouldBe(_settings.PresencePenalty);
        options.FrequencyPenalty.ShouldBe(_settings.FrequencyPenalty);
    }

    [Test]
    public async Task GetUnknownWordSuggestionsAsync_WithEmptyList_ReturnsEmptyArray()
    {
        // Arrange
        var unknownWords = Array.Empty<string>();
        var cancellationToken = CancellationToken.None;

        // Act
        var results = await _sut.GetUnknownWordSuggestionsAsync(unknownWords, cancellationToken);

        // Assert
        results.ShouldBeEmpty();
        await _chatClient.DidNotReceive().CompleteChatAsync(
            Arg.Any<IEnumerable<ChatMessage>>(),
            Arg.Any<ChatCompletionOptions>(),
            Arg.Any<CancellationToken>());
    }

    #pragma warning disable OPENAI001
    private ClientResult<ChatCompletion> CreateResponseWithText(string resolvedSuggestion)
    {
        var completion = OpenAIChatModelFactory.ChatCompletion(
            content: new ChatMessageContent(ChatMessageContentPart.CreateTextPart(resolvedSuggestion)));

        var pipelineResponse = Substitute.For<PipelineResponse>();
        var completionResponse = ClientResult.FromValue(completion, pipelineResponse);
        return completionResponse;
    }
    #pragma warning restore OPENAI001
}