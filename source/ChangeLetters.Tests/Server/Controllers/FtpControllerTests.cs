using ChangeLetters.Shared;
using ChangeLetters.Domain.IO;
using ChangeLetters.Application;
using ChangeLetters.Application.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeLetters.Tests.Server.Controllers;

[TestFixture]
public class FtpControllerTests
{
    private FtpController _sut = null!;
    private Configuration _config = null!;
    private IConfigurationIo _configIo = null!;
    private IFtpInteractor _ftpConnector = null!;
    private ILogger<FtpController> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _config = new Configuration();
        _configIo = Substitute.For<IConfigurationIo>();
        _ftpConnector = Substitute.For<IFtpInteractor>();
        _logger = Substitute.For<ILogger<FtpController>>();
        var httpContext = new DefaultHttpContext() { RequestAborted = CancellationToken.None };

        _configIo.GetConfiguration().Returns(_config);

        _sut = new FtpController(_configIo, _ftpConnector, _logger)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
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
        var renameResult = new RenameFileItemsResult{Succeeded = true};
        _ftpConnector.RenameItemsAsync(req.Folder, req.FileItemType, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(renameResult);
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
        var entries = new List<VocabularyEntry> { new () { UnknownWord = "foo" } };
        _ftpConnector.ReadUnknownWordsAsync("folder", Arg.Any<CancellationToken>()).Returns(entries);
        var result = await _sut.ReadUnknownWordsAsync("folder");
        result.Result.ShouldBeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.ShouldBe(entries);
    }
}
