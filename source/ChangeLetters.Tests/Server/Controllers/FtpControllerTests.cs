using ChangeLetters.DTOs;
using ChangeLetters.Model;
using ChangeLetters.Handlers;
using ChangeLetters.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ChangeLetters.Tests.Server.Controllers;

[TestFixture]
public class FtpControllerTests
{
    private FtpController _sut = null!;
    private Configuration _config = null!;
    private IFtpHandler _ftpHandler = null!;
    private IConfigurationIo _configIo = null!;
    private IFtpConnector _ftpConnector = null!;
    private ILogger<FtpController> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _ftpHandler = Substitute.For<IFtpHandler>();
        _ftpConnector = Substitute.For<IFtpConnector>();
        _configIo = Substitute.For<IConfigurationIo>();
        _logger = Substitute.For<ILogger<FtpController>>();
        _config = new Configuration();
        _configIo.GetConfiguration().Returns(_config);
        _sut = new FtpController(_ftpHandler, _configIo, _ftpConnector, _logger);
    }

    [Test]
    public async Task ConnectAsync_ReturnsOkWithResult()
    {
        _ftpConnector.ConnectAsync(_config).Returns(Task.FromResult(true));
        var result = await _sut.ConnectAsync(_config);
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(true);
    }

    [Test]
    public async Task ReadFoldersAsync_ReturnsOkWithFolders()
    {
        var folders = new[] { new FileItem { Name = "folder", IsFolder = true, FullName = "./folder"} };
        _ftpConnector.ReadFoldersAsync(_config, "/", Arg.Any<CancellationToken>()).Returns(folders);
        var result = await _sut.ReadFoldersAsync("%2F");
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(folders);
    }

    [Test]
    public async Task RenameItemsAsync_ReturnsOkWithResult()
    {
        var req = new RenameFileItemsRequest { Folder = "folder", FileItemType = 0 };
        var renameResult = new RenameFileItemsResult { Succeeded = true };
        _ftpHandler.RenameItemsAsync(req.Folder, req.FileItemType, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(renameResult);
        var result = await _sut.RenameItemsAsync(req);
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(renameResult);
    }

    [Test]
    public async Task CheckQuestionMarksAsync_ReturnsOkWithStatus()
    {
        var fileItem = new FileItem { FullName = "folder", IsFolder = true, Name = "folder" };
        fileItem.FolderStatus = FolderStatus.HasQuestionMarks;
        await _sut.CheckQuestionMarksAsync(fileItem);
        await _ftpConnector.Received().CheckQuestionMarksAsync(fileItem, _config, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ReadUnknownWordsAsync_ReturnsOkWithEntries()
    {
        var entries = new List<VocabularyEntry> { new VocabularyEntry { UnknownWord = "foo" } };
        _ftpHandler.ReadUnknownWordsAsync("folder", Arg.Any<CancellationToken>()).Returns(entries);
        var result = await _sut.ReadUnknownWordsAsync("folder");
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(entries);
    }
}
