namespace ChangeLetters.Application;

/// <summary>Interface IFtpInteractor.</summary>
public interface IFtpInteractor
{
    /// <summary>Rename items as an asynchronous operation.</summary>
    /// <param name="folder">The folder.</param>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="connectionId">The connection identifier.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<RenameFileItemsResult> RenameItemsAsync(string folder, FileItemType fileItemType, string? connectionId, CancellationToken token);

    /// <summary>Read unknown words as an asynchronous operation.</summary>
    /// <param name="folder">The folder.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<IList<VocabularyEntry>> ReadUnknownWordsAsync(string folder, CancellationToken token);

    /// <summary>Connect as an asynchronous operation.</summary>
    /// <param name="config">The configuration.</param>
    /// <returns>See description.</returns>
    Task<bool> ConnectAsync(Configuration config);

    /// <summary>Check question marks as an asynchronous operation.</summary>
    /// <param name="fileItem">The file item.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task CheckQuestionMarksAsync(FileItem fileItem, Configuration config, CancellationToken cancellationToken);

    /// <summary>Read folders as an asynchronous operation.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<FileItem[]> ReadFoldersAsync(Configuration config, string folder, CancellationToken token);
}