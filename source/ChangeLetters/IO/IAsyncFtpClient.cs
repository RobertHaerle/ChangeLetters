using FluentFTP;

namespace ChangeLetters.IO;

/// <summary>Interface IAsyncFtpClient.
/// Extends the <see cref="System.IAsyncDisposable" />
/// Extends the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="System.IAsyncDisposable" />
/// <seealso cref="System.IDisposable" />
public interface IAsyncFtpClient : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.Port"/>.
    /// </summary>
    int Port { get; set; }

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.Host"/>.
    /// </summary>
    string Host { get; set; }

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.Credentials"/>.
    /// </summary>
    System.Net.NetworkCredential Credentials { get; set; }

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.AutoDetect()"/>.
    /// </summary>
    Task<List<FtpProfile>> AutoDetect();

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.Connect(System.Threading.CancellationToken)"/>.
    /// </summary>
    Task Connect(CancellationToken token = default);

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.MoveDirectory(string, string, FluentFTP.FtpRemoteExists, System.Threading.CancellationToken)"/>.
    /// </summary>
    Task<bool> MoveDirectory(string path, string dest);

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.AutoConnect(System.Threading.CancellationToken)"/>.
    /// </summary>
    Task AutoConnect(CancellationToken token = default);

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.GetNameListing(string, System.Threading.CancellationToken)"/>.
    /// </summary>
    Task<string[]> GetNameListing(string path, CancellationToken token = default);

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.GetListing(string, FluentFTP.FtpListOption, System.Threading.CancellationToken)"/>.
    /// </summary>
    Task<FtpListItem[]> GetListing(string path, FtpListOption options, CancellationToken token = default);

    /// <summary>
    /// See <see cref="FluentFTP.AsyncFtpClient.MoveFile(string, string, FluentFTP.FtpRemoteExists, System.Threading.CancellationToken)"/>.
    /// </summary>
    Task<bool> MoveFile(string path, string dest, FtpRemoteExists existsMode, CancellationToken token = default);
}