using ChangeLetters.Shared;

namespace ChangeLetters.Domain.Ftp;

/// <summary>Interface IFtpConnector.</summary>
internal interface IFtpConnector
{
    /// <summary>Connect as an asynchronous operation.</summary>
    /// <param name="config">The configuration.</param>
    /// <returns>See description.</returns>
    Task<bool> ConnectAsync(Configuration config);

    /// <summary>Read folders of a directory as an asynchronous operation.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<FtpItem[]> ReadFoldersAsync(Configuration config, string folder, CancellationToken token);

    /// <summary>Read files of a directory as an asynchronous operation.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<List<Shared.FileItem>> ReadFilesAsync(Configuration config, string folder, CancellationToken token);

    /// <summary>Rename file as an asynchronous operation.</summary>
    /// <param name="fileItem">The file item.</param>
    /// <param name="newName">The new name.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<bool> RenameFileAsync(Shared.FileItem fileItem, string newName, Configuration config, CancellationToken cancellationToken);

    /// <summary>Rename folder as an asynchronous operation.</summary>
    /// <param name="folderItem">The folder item.</param>
    /// <param name="newName">The new name.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>See description.</returns>
    Task<bool> RenameFolderAsync(Shared.FileItem folderItem, string newName, Configuration config,
        CancellationToken cancellationToken);

    /// <summary>Check for question marks in any name in the folder structure as an asynchronous operation.</summary>
    /// <param name="fileItem">The file item to start with.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task CheckQuestionMarksAsync(Shared.FileItem fileItem, Configuration config,
        CancellationToken cancellationToken = default);
}