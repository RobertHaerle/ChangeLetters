using System.Net;
using ChangeLetters.Domain.Connectors;
using ChangeLetters.Domain.Handlers;
using ChangeLetters.Domain.IO;

namespace ChangeLetters.Application.Http.Controllers;

/// <summary> 
/// Executes FTP commands.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FtpController : ControllerBase
{
    private readonly IFtpHandler _ftpHandler;
    private readonly IFtpConnector _ftpConnector;
    private readonly ILogger<FtpController> _log;
    private readonly Configuration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpController"/> class.
    /// </summary>
    /// <param name="ftpHandler">The FTP handler.</param>
    /// <param name="configIo">The configuration io.</param>
    /// <param name="ftpConnector">The FTP connector.</param>
    /// <param name="log">The log.</param>
    public FtpController(
        IFtpHandler ftpHandler,
        IConfigurationIo configIo,
        IFtpConnector ftpConnector,
        ILogger<FtpController> log)
    {
        _log = log;
        _ftpHandler = ftpHandler;
        _ftpConnector = ftpConnector;
        _configuration = configIo.GetConfiguration();
    }

    /// <summary>Checks FTP connection as described in <paramref name="config"/> as an asynchronous operation.</summary>
    /// <param name="config">The configuration.</param>
    /// <returns>true or false.</returns>
    [HttpPost("connect")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ConnectAsync([FromBody] Configuration config)
    {
        _log.LogInformation("Connect called with Host: {Host}, Port: {Port}, User: {User}", config.HostName, config.Port, config.UserName);
        var result = await _ftpConnector.ConnectAsync(config);
        _log.LogInformation("Connect result: {Result}", result);
        return Ok(result);
    }

    /// <summary>Read folders of a directory as an asynchronous operation. No tree read is done.</summary>
    /// <param name="folder">The folder.</param>
    /// <returns>the folders in the specified directory.</returns>
    [HttpGet("read-folders/{folder}")]
    [ProducesResponseType<FileItem[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<FileItem[]>> ReadFoldersAsync(string folder)
    {
        folder = WebUtility.UrlDecode(folder);
        _log.LogInformation("ReadFolders called for folder: {Folder}", folder);
        FileItem[] result;
        try
        {
            result = await _ftpConnector.ReadFoldersAsync(_configuration, folder, HttpContext.RequestAborted);
        }
        catch (TaskCanceledException)
        {
            result = [];
        }
        _log.LogInformation("ReadFolders returned {Total} items", result.Length);
        return Ok(result);
    }

    /// <summary>Rename items as an asynchronous operation.</summary>
    /// <param name="request">The request.</param>
    /// <returns>Ok result with the success flag.</returns>
    [HttpPost("rename-items")]
    [ProducesResponseType<RenameFileItemsResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<RenameFileItemsResult>> RenameItemsAsync([FromBody] RenameFileItemsRequest request)
    {
        _log.LogInformation("RenameItems called for {Folder} ", request.Folder);
        var result = await _ftpHandler.RenameItemsAsync(request.Folder, request.FileItemType, request.SignalRConnectionId, HttpContext.RequestAborted);
        _log.LogInformation("RenameItems for {Folder} resulted in {Success}", request.Folder, result.Succeeded ? "succeeded" : "failed");
        return Ok(result);
    }

    /// <summary>Check for question marks in any name in the folder structure as an asynchronous operation.</summary>
    /// <param name="fileItem">The file item.</param>
    /// <returns>See description.</returns>
    [HttpPut("check-question-marks")]
    [ProducesResponseType<FolderStatus>(StatusCodes.Status200OK)]
    public async Task<ActionResult<FolderStatus>> CheckQuestionMarksAsync([FromBody] FileItem fileItem)
    {
        _log.LogInformation("CheckQuestionMarks called for {FullName}", fileItem.FullName);
        await _ftpConnector.CheckQuestionMarksAsync(fileItem, _configuration, HttpContext.RequestAborted);
        _log.LogInformation("CheckQuestionMarks completed for {FullName}. Found question marks in file names: {HasQuestionMarks}",
            fileItem.FullName, 
            fileItem.FolderStatus == FolderStatus.HasQuestionMarks);
        return Ok(fileItem.FolderStatus);
    }

    /// <summary>Read unknown words as an asynchronous operation.</summary>
    /// <param name="folder">The full name.</param>
    /// <returns>List of words unknown to the OS.</returns>
    [HttpGet("read-unknown-words/{folder}")]
    [ProducesResponseType<List<VocabularyEntry>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VocabularyEntry>>> ReadUnknownWordsAsync(string folder)
    {
        folder = WebUtility.UrlDecode(folder);
        _log.LogInformation("ReadUnknownWords called for {folder}", folder);
        var unknownEntries = await _ftpHandler.ReadUnknownWordsAsync(folder, HttpContext.RequestAborted);
        _log.LogInformation("ReadUnknownWords returned {Total} words", unknownEntries.Count);
        return Ok(unknownEntries);
    }
}