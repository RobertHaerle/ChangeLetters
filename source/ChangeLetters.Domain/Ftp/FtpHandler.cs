using ChangeLetters.Database.Repositories;
using ChangeLetters.Domain.Handlers;
using ChangeLetters.Domain.IO;
using ChangeLetters.Domain.ParseLogic;
using ChangeLetters.Domain.Pocos;
using ChangeLetters.Shared;

namespace ChangeLetters.Domain.Ftp;

/// <summary> 
/// Class FtpHandler.
/// Implements <see cref="IFtpHandler" />
/// </summary>
[Export(typeof(IFtpHandler))]
internal class FtpHandler : IFtpHandler
{
    private readonly IFileParser _fileParser;
    private readonly ILogger<FtpHandler> _log;
    private readonly IFtpConnector _ftpConnector;
    private readonly Configuration _configuration;
    private readonly IVocabularyHandler _vocabularyHandler;
    private readonly IVocabularyRepository _vocabularyRepository;
    private readonly ISignalRRenameHandler _signalRRenameHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpHandler"/> class.
    /// </summary>
    /// <param name="fileParser">The file parser.</param>
    /// <param name="log">The log.</param>
    /// <param name="ftpConnector">The FTP connector.</param>
    /// <param name="configurationIo">The configuration io.</param>
    /// <param name="vocabularyHandler">The vocabulary handler.</param>
    /// <param name="vocabularyRepository">The vocabulary repository.</param>
    /// <param name="signalRRenameHandler">The signal r rename handler.</param>
    public FtpHandler(
        IFileParser fileParser,
        ILogger<FtpHandler> log,
        IFtpConnector ftpConnector,
        IConfigurationIo configurationIo,
        IVocabularyHandler vocabularyHandler,
        IVocabularyRepository vocabularyRepository,
        ISignalRRenameHandler signalRRenameHandler)
    {
        _log = log;
        _fileParser = fileParser;
        _ftpConnector = ftpConnector;
        _vocabularyHandler = vocabularyHandler;
        _vocabularyRepository = vocabularyRepository;
        _signalRRenameHandler = signalRRenameHandler;
        _configuration = configurationIo.GetConfiguration();
    }

    /// <inheritdoc />
    public async Task<HandlerResult<string>> RenameItemsAsync(string folder, FileItemType fileItemType, string? connectionId, CancellationToken token)
    {
        _log.LogInformation("RenameItems called for {Folder} ", folder);
        try
        {
            var fileItems = await _ftpConnector.ReadFilesAsync(_configuration, folder, token);

            var itemsToBeChanged = fileItems
                .Where(f=> ConsiderFileItemType(f, fileItemType))
                .Where(f => f.Name.Contains('?'))
                .OrderByDescending(f => f.FullName.Length)
                .ToArray();

            await _signalRRenameHandler.SendMaxChangesAsync(fileItemType, itemsToBeChanged.Length, connectionId);
            var vocabularyItems = await _vocabularyRepository.GetAllItemsAsync(token)
                .ConfigureAwait(false);
            var vocabulary = vocabularyItems.ToDictionary(x => x.UnknownWord);
            var counter = 0;
            foreach (var fileItem in itemsToBeChanged)
            {
                await _signalRRenameHandler.SendCurrentItemNumberAsync(fileItemType, ++counter, connectionId);
                var result = false;
                if (_fileParser.TryReplaceUnknownWordsInName(fileItem, vocabulary, out var newFileItem))
                    result = fileItem.IsFolder
                        ? await _ftpConnector.RenameFolderAsync(fileItem, newFileItem.FullName, _configuration, token)
                        : await _ftpConnector.RenameFileAsync(fileItem, newFileItem.FullName, _configuration, token);
                if (!result)
                {
                    _log.LogWarning("RenameItems failed for {FullName}", fileItem.FullName);
                    return HandlerResult<string>.Failure(fileItem.FullName);
                }
            }
            _log.LogInformation("RenameItems completed for all items");
            return HandlerResult<string>.Success();
        }
        catch (TaskCanceledException)
        {
            return HandlerResult<string>.Failure("Operation canceled");
        }
    }

    private bool ConsiderFileItemType(Shared.FileItem fileItem, FileItemType requiredFileItemType)
    => fileItem.IsFolder && requiredFileItemType == FileItemType.Folder
        || !fileItem.IsFolder && requiredFileItemType == FileItemType.File;

    /// <inheritdoc />
    public async Task<List<RequiredVocabulary>> ReadUnknownWordsAsync(string folder, CancellationToken token)
    {
        try
        {
            var allFileItems = await _ftpConnector.ReadFilesAsync(_configuration, folder, token)
                .ConfigureAwait(false);
            var unknownWordsInFolder = _fileParser.GetWordsWithUnknownLetters(allFileItems);
            if (unknownWordsInFolder.Any())
                return await _vocabularyHandler.GetRequiredVocabularyAsync(unknownWordsInFolder, token)
                    .ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
        }

        return [];
    }
}