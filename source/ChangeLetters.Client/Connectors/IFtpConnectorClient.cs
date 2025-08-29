using ChangeLetters.DTOs;

namespace ChangeLetters.Client.Connectors;

/// <summary>Interface IFtpConnectorClient.</summary>
public interface IFtpConnectorClient
{
    /// <summary>
    /// Establishes a connection to the FTP server using the specified configuration.
    /// </summary>
    /// <param name="config">The configuration to use for the connection.</param>
    /// <returns>True if the connection was successful; otherwise, false.</returns>
    Task<bool> ConnectAsync(Configuration config);
    
    /// <summary>Check for question marks in any name in the folder structure as an asynchronous operation.</summary>
    /// <param name="fileItem">The file to check.</param>
    /// <param name="token">Optional cancellation token.</param>
    Task CheckQuestionMarksAsync(FileItem fileItem, CancellationToken token);

    /// <summary>Read unknown words as an asynchronous operation.</summary>
    /// <param name="folder">The folder.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<Dictionary<string, VocabularyEntry>> ReadUnknownWordsAsync(string folder, CancellationToken token);

    /// <summary>Rename items as an asynchronous operation.</summary>
    /// <param name="request">The request.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<RenameFileItemsResult> RenameItemsAsync(RenameFileItemsRequest request, CancellationToken token);

    /// <summary>Read folders as an asynchronous operation.</summary>
    /// <param name="folder">The folder.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>See description.</returns>
    Task<FileItem[]> ReadFoldersAsync(string folder, CancellationToken token);
}
