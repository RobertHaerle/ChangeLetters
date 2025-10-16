namespace ChangeLetters.FTP;

/// <summary>Interface IAsyncFtpClient.
/// Extends <see cref="IAsyncDisposable" />
/// Extends <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IAsyncDisposable" />
/// <seealso cref="IDisposable" />
public interface IAsyncFtpClient : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Corresponds to the <c>Port</c> property of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    int Port { get; set; }

    /// <summary>
    /// Corresponds to the <c>Host</c> property of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    string Host { get; set; }

    /// <summary>
    /// Corresponds to the <c>Credentials</c> property of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    System.Net.NetworkCredential Credentials { get; set; }

    /// <summary>
    /// Corresponds to the <c>AutoDetect()</c> method of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    Task<List<FtpProfile>> AutoDetect();

    /// <summary>
    /// Corresponds to the <c>Connect(CancellationToken)</c> method of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    Task Connect(CancellationToken token = default);

    /// <summary>
    /// Corresponds to the <c>MoveDirectory(string, string, FtpRemoteExists, CancellationToken)</c> method of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    Task<bool> MoveDirectory(string path, string dest);

    /// <summary>
    /// Corresponds to the <c>AutoConnect(CancellationToken)</c> method of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    Task AutoConnect(CancellationToken token = default);

    /// <summary>
    /// Corresponds to the <c>GetNameListing(string, CancellationToken)</c> method of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    Task<string[]> GetNameListing(string path, CancellationToken token = default);

    /// <summary>
    /// Corresponds to the <c>GetListing(string, FtpListOption, CancellationToken)</c> method of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    Task<FtpListItem[]> GetListing(string path, FtpListOption options, CancellationToken token = default);

    /// <summary>
    /// Corresponds to the <c>MoveFile(string, string, FtpRemoteExists, CancellationToken)</c> method of <c>FluentFTP.AsyncFtpClient</c>.
    /// </summary>
    Task<bool> MoveFile(string path, string dest, FtpRemoteExists existsMode, CancellationToken token = default);
}