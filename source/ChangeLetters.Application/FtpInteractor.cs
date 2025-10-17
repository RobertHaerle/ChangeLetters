using ChangeLetters.Domain.Ftp;
using ChangeLetters.Domain.Pocos;

namespace ChangeLetters.Application;

/// <summary> 
/// Class FtpInteractor.
/// Implements <see cref="IFtpInteractor" />
/// </summary>
[Export(typeof(IFtpInteractor))]
internal class FtpInteractor(
    IFtpHandler _ftpHandler,
    IFtpConnector _ftpConnector
    ) : IFtpInteractor
{
    /// <inheritdoc />
    public async Task<RenameFileItemsResult> RenameItemsAsync(string folder, FileItemType fileItemType, string? connectionId, CancellationToken token)
    {
        var result = await _ftpHandler.RenameItemsAsync(folder, fileItemType, connectionId, token).ConfigureAwait(false);
        return new()
        {
            Succeeded = result.Succeeded,
            FailedFile = result.ErrorMessage
        };
    }

    /// <inheritdoc />
    public async Task<IList<VocabularyEntry>> ReadUnknownWordsAsync(string folder, CancellationToken token)
    {
        var result = await _ftpHandler.ReadUnknownWordsAsync(folder, token).ConfigureAwait(false);
        return result.Select(x => x.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public Task<bool> ConnectAsync(Configuration config)
        => _ftpConnector.ConnectAsync(config);

    /// <inheritdoc />
    public Task CheckQuestionMarksAsync(FileItem fileItem, Configuration config, CancellationToken cancellationToken)
        => _ftpConnector.CheckQuestionMarksAsync(fileItem, config, cancellationToken);

    /// <inheritdoc />
    public async Task<FileItem[]> ReadFoldersAsync(Configuration config, string folder, CancellationToken token)
    {
        var items = await _ftpConnector.ReadFoldersAsync(config, folder, token).ConfigureAwait(false);

        return items.Select(x=> x.ToFileItem()).ToArray();
    }
}