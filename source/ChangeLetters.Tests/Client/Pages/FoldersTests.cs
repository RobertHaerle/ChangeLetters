using Bunit;
using Blazored.Modal;
using ChangeLetters.Client.Pages;
using ChangeLetters.Client.Connectors;
using ChangeLetters.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using TestContext = Bunit.TestContext;

namespace ChangeLetters.Tests.Client.Pages;

public class FoldersTests : TestContext
{
    [SetUp]
    public void Setup()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var ftpConnector = Substitute.For<IFtpConnectorClient>();
        var vocabularyConnector = Substitute.For<IVocabularyConnector>();
        var signalRRenameConnector = Substitute.For<ISignalRRenameConnector>();

        ftpConnector.ReadFoldersAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new FileItem[]
            {
                new FileItem { Name = "TestFolder", FullName = "/TestFolder", IsFolder = true, FolderStatus = FolderStatus.Ok }
            }));

        Services.AddBlazoredModal();
        Services.AddFluentUIComponents();
        Services.AddSingleton(ftpConnector);
        Services.AddSingleton(vocabularyConnector);
        Services.AddSingleton(signalRRenameConnector);
    }

    [Test]
    public void FoldersPage_ShouldRenderCorrectly()
    {
        var component = RenderComponent<Folders>();
        var pageTitle = component.Find("h5");
        pageTitle.MarkupMatches("<h5 class=\"modal-title\">Select folder</h5>");
    }
}