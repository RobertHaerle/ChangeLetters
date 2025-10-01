using ChangeLetters.Shared;

namespace ChangeLetters.Domain.Handlers;

/// <summary>Interface IFtpHandler.</summary>
public interface IFtpHandler
{
    /// <summary>Rename items as an asynchronous operation.</summary>
    /// <param name="folder">The folder.</param>
    /// <param name="fileItemType">Type of the file item.</param>
    /// <param name="connectionId">The connection identifier.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A <see cref="RenameFileItemsResult" />.</returns>
    Task<RenameFileItemsResult> RenameItemsAsync(string folder, FileItemType fileItemType, string? connectionId, CancellationToken token);

    /// <summary>Read unknown words as an asynchronous operation.</summary>
    /// <param name="folder">The folder.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>List of words unknown to the OS.</returns>
    Task<List<VocabularyEntry>> ReadUnknownWordsAsync(string folder, CancellationToken token);
}