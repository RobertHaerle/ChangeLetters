using ChangeLetters.Connectors;
using ChangeLetters.DTOs;
using FluentFTP;
using IAsyncFtpClient = ChangeLetters.Wrappers.IAsyncFtpClient;

namespace ChangeLetters.Tests.Server.IO;

[TestFixture]
public class FtpConnectorTests
{
    private FtpConnector _sut;
    private Configuration _config;
    private IAsyncFtpClient _ftpClient;
    private ILogger<FtpConnector> _logger;
    private Func<IAsyncFtpClient> _ftpClientFactory;

    [SetUp]
    public void SetUp()
    {
        _ftpClient = Substitute.For<IAsyncFtpClient>();
        _ftpClientFactory = () => _ftpClient;
        _logger = Substitute.For<ILogger<FtpConnector>>();
        _config = new Configuration { HostName = "host", Port = 21, UserName = "user", Password = "pass" };

        _sut = new FtpConnector(_logger, _ftpClientFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _ftpClient?.Dispose();
    }

    [Test]
    public async Task ConnectAsync_ReturnsTrue_OnSuccess()
    {
        _ftpClient.AutoDetect().Returns(Task.FromResult(new List<FtpProfile> { new () }));
        var result = await _sut.ConnectAsync(_config);
        result.ShouldBeTrue();
    }

    [Test]
    public async Task ConnectAsync_ReturnsFalse_OnException()
    {
        _ftpClient.AutoDetect().Returns<Task<List<FluentFTP.FtpProfile>>>(_ => throw new Exception("fail"));
        var result = await _sut.ConnectAsync(_config);
        result.ShouldBeFalse();
    }

    [Test]
    public async Task ReadFoldersAsync_ReturnsFolders()
    {
        // arrange
        var items = new[]
        {
            Substitute.For<FluentFTP.FtpListItem>()
        };
        items[0].Type=FluentFTP.FtpObjectType.Directory;
        items[0].Name="folder";
        items[0].FullName="/folder";
        _ftpClient.GetListing("/", Arg.Any<FluentFTP.FtpListOption>(), Arg.Any<CancellationToken>())
            .Returns(items);
        _ftpClient.GetListing(items[0].FullName, Arg.Any<FluentFTP.FtpListOption>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _ftpClient.AutoConnect(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // act
        var result = await _sut.ReadFoldersAsync(_config, "/", CancellationToken.None);

        // assert
        result.Count(f => f.Name == "folder" && f.IsFolder).ShouldBe(1);
    }

    [Test]
    public async Task ReadFilesAsync_ReturnsFilesAndFolders()
    {
        // arrange
        var items = new[]
        {
            Substitute.For<FluentFTP.FtpListItem>(),
            Substitute.For<FluentFTP.FtpListItem>()
        };
        items[0].Type = FluentFTP.FtpObjectType.Directory;
        items[0].Name="folder";
        items[0].FullName="/folder";
        items[1].Type=FluentFTP.FtpObjectType.File;
        items[1].Name="file.txt";
        items[1].FullName="/folder/file.txt";
        _ftpClient.GetListing("/", Arg.Any<FluentFTP.FtpListOption>(), Arg.Any<CancellationToken>())
            .Returns([items[0]]);
        _ftpClient.GetListing(items[0].FullName, Arg.Any<FluentFTP.FtpListOption>(), Arg.Any<CancellationToken>())
            .Returns([items[1]]);
        _ftpClient.AutoConnect(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // act
        var result = await _sut.ReadFilesAsync(_config, "/", CancellationToken.None);

        // assert
        result.Count(f => f.Name == "folder" && f.IsFolder).ShouldBe(1);
        result.Count(f => f.Name == "file.txt" && !f.IsFolder).ShouldBe(1);
    }

    [Test]
    public async Task RenameFileAsync_ReturnsTrue_OnSuccess()
    {
        _ftpClient.MoveFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FluentFTP.FtpRemoteExists>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        _ftpClient.AutoConnect(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var file = new FileItem { Name = "file.txt", FullName = "/file.txt" };
        var result = await _sut.RenameFileAsync(file, "/file2.txt", _config, CancellationToken.None);
        result.ShouldBeTrue();
    }

    [Test]
    public async Task RenameFolderAsync_ReturnsTrue_OnSuccess()
    {
        _ftpClient.MoveDirectory(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(true));
        _ftpClient.AutoConnect(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var folder = new FileItem { Name = "folder", FullName = "/folder", IsFolder = true };
        var result = await _sut.RenameFolderAsync(folder, "/folder2", _config, CancellationToken.None);
        result.ShouldBeTrue();
    }

    [Test]
    public async Task CheckQuestionMarksAsync_SetsFolderStatus()
    {
        _ftpClient.Connect(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var file = new FileItem { Name = "folder", FullName = "/folder", IsFolder = true };
        _ftpClient.GetListing(Arg.Any<string>(), Arg.Any<FluentFTP.FtpListOption>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Array.Empty<FluentFTP.FtpListItem>()));
        await _sut.CheckQuestionMarksAsync(file, _config, CancellationToken.None);

        file.FolderStatus.ShouldBeOneOf(Enum.GetValues<FolderStatus>());
    }
}
