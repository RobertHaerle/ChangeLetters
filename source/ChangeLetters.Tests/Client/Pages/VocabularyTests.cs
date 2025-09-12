using Bunit;
using ChangeLetters.DTOs;
using ChangeLetters.Client.Pages;
using ChangeLetters.Client.Connectors;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Bunit.TestContext;

namespace ChangeLetters.Tests.Client.Pages;

public class VocabularyTests : TestContext
{
    [SetUp]
    public void Setup()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/Microsoft.AspNetCore.Components.QuickGrid/QuickGrid.razor.js");

        var vocabularyConnector = Substitute.For<IVocabularyConnector>();
        vocabularyConnector.GetAllItemsAsync().Returns(Task.FromResult(new Dictionary<string, VocabularyEntry>
        {
            { "word1", new VocabularyEntry { UnknownWord = "word1", CorrectedWord = "word1" } }
        }));

        Services.AddSingleton(vocabularyConnector);
    }

    [Test]
    public void VocabularyPage_ShouldRenderCorrectly()
    {
        var component = RenderComponent<Vocabulary>();
        var header = component.Find("h3");
        header.MarkupMatches("<h3>Vocabulary</h3>");
    }
}