using FluentFTP;
using FluentFTP.Logging;

namespace ChangeLetters.Wrappers;

/// <summary> 
/// Class AsyncFtpClientWrapper.
/// Implements <see cref="IAsyncFtpClient" />
/// </summary>
[Export(typeof(IAsyncFtpClient))]
public class AsyncFtpClientWrapper : IAsyncFtpClient
{
    private readonly AsyncFtpClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFtpClientWrapper"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="logAdapter">The log adapter.</param>
    public AsyncFtpClientWrapper(
        AsyncFtpClient client,
        FtpLogAdapter logAdapter)
    {
        _client = client;
        _client.Logger = logAdapter;
    }

    /// <inheritdoc />
    public int Port { get => _client.Port; set => _client.Port = value; }

    /// <inheritdoc />
    public string Host { get => _client.Host; set => _client.Host = value; }

    /// <inheritdoc />
    public System.Net.NetworkCredential Credentials { get => _client.Credentials; set => _client.Credentials = value; }

    /// <inheritdoc />
    public Task<List<FtpProfile>> AutoDetect() 
        => _client.AutoDetect();

    /// <inheritdoc />
    public Task AutoConnect(CancellationToken token = default) 
        => _client.AutoConnect(token);

    /// <inheritdoc />
    public Task<FtpListItem[]> GetListing(string path, FtpListOption options, CancellationToken token = default) 
        => _client.GetListing(path, options, token);

    /// <inheritdoc />
    public Task<string[]> GetNameListing(string path, CancellationToken token = default) 
        => _client.GetNameListing(path, token);

    /// <inheritdoc />
    public Task<bool> MoveFile(string path, string dest, FtpRemoteExists existsMode, CancellationToken token = default) 
        => _client.MoveFile(path, dest, existsMode, token);

    /// <inheritdoc />
    public Task<bool> MoveDirectory(string path, string dest) 
        => _client.MoveDirectory(path, dest);

    /// <inheritdoc />
    public Task Connect(CancellationToken token = default) 
        => _client.Connect(token);

    /// <inheritdoc />
    public ValueTask DisposeAsync()
        => _client.DisposeAsync();

    /// <inheritdoc />
    public void Dispose()
        => _client.Dispose();
}