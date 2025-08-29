using ChangeLetters.DTOs;
using FluentFTP;
using ChangeLetters.Model;
using FluentFTP.Exceptions;

namespace ChangeLetters.IO;

/// <summary> 
/// Class FtpConnector.
/// Implements <see cref="IFtpConnector" />
/// </summary>
[Export(typeof(IFtpConnector))]
public class FtpConnector : IFtpConnector
{
    private readonly ILogger<FtpConnector> _log;
    private readonly Func<IAsyncFtpClient> _getNewFtpClient;

    public FtpConnector(
        ILogger<FtpConnector> log,
        Func<IAsyncFtpClient> getNewFtpClient)
    {
        _log = log;
        _getNewFtpClient = getNewFtpClient;
    }

    /// inheritdoc />
    public async Task<bool> ConnectAsync(Configuration config)
    {
        await using var ftpClient = GetFtpClient(config);

        try
        {
            await ftpClient.AutoDetect();
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error connecting to FTP server.");
            return false;
        }
    }

    /// inheritdoc />
    public async Task<FileItem[]> ReadFoldersAsync(Configuration config, string folder, CancellationToken token)
    {
        await using var ftpClient = GetFtpClient(config);

        try
        {
            await ftpClient.AutoConnect(token);
            var resultSet = await ftpClient.GetListing(folder, FtpListOption.AllFiles, token);
            var folders = resultSet
                .Where(item => item.Type == FtpObjectType.Directory)
                .Select(x => new FileItem { Name = x.Name, FullName = x.FullName, IsFolder = true })
                .ToArray();

            return folders;
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
                _log.LogError(ex, "Error reading folders");
            return [];
        }
    }

    /// <inheritdoc />
    public async Task CheckQuestionMarksAsync(FileItem fileItem, Configuration config, CancellationToken cancellationToken)
    {
        await using var ftpClient = GetFtpClient(config);
        try
        {
            await ftpClient.Connect(cancellationToken);
            _log.LogInformation("read {folder}", fileItem.FullName);
            var hasQuestionMarks = await HasQuestionMarksAsync(fileItem.FullName, ftpClient, cancellationToken);
            fileItem.FolderStatus = hasQuestionMarks
                ? FolderStatus.HasQuestionMarks
                : FolderStatus.Ok;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error checking for question marks in folder {Folder}", fileItem.FullName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<FileItem>> ReadFilesAsync(Configuration config, string folder, CancellationToken token)
    {
        await using var ftpClient = GetFtpClient(config);

        try
        {
            await ftpClient.AutoConnect(token);
            var allEntries = new List<FileItem>();
            await AddItemsToListAsync(folder, allEntries, ftpClient, token);
            return allEntries;
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
                _log.LogError(ex, "Error reading files");
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<bool> RenameFileAsync(FileItem fileItem, string newName, Configuration config, CancellationToken cancellationToken)
    {
        await using var ftpClient = GetFtpClient(config);

        try
        {
            await ftpClient.AutoConnect(cancellationToken);
            return await ftpClient.MoveFile(fileItem.FullName, newName, FtpRemoteExists.Overwrite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (FtpCommandException ex)
        {
            if (ex.CompletionCode == "500" && await CheckExistenceAsync(newName, ftpClient, cancellationToken))
            {
                _log.LogError(ex, "File {newFileName} already exists", newName);
                return true;
            }

            _log.LogError(ex, "Error renaming file");
            return false;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error renaming file");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RenameFolderAsync(FileItem folderItem, string newName, Configuration config, CancellationToken cancellationToken)
    {
        await using var ftpClient = GetFtpClient(config);

        try
        {
            await ftpClient.AutoConnect(cancellationToken);
            return await ftpClient.MoveDirectory(folderItem.FullName, newName);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error renaming folder");
            return false;
        }
    }

    private async Task<bool> HasQuestionMarksAsync(string fullName, IAsyncFtpClient ftpClient, CancellationToken cancellationToken)
    {
        if (fullName.Contains('?'))
            return true;
        int counter = 0;
        bool succeeded = false;
        while (!succeeded)
        {
            if (cancellationToken.IsCancellationRequested)
                return false;
            try
            {
                var resultSet = await ftpClient.GetListing(fullName, FtpListOption.AllFiles, cancellationToken);
                if (resultSet.Select(f => f.FullName).Any(f => f.Contains('?')))
                    return true;
                var folders = resultSet.Where(item => item.Type == FtpObjectType.Directory);
                foreach (var folder in folders)
                {
                    if (await HasQuestionMarksAsync(folder.FullName, ftpClient, cancellationToken))
                        return true;
                }
                succeeded = true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error checking for question marks in folder {Folder}", fullName);
                if (counter < 2)
                {
                    counter++;
                    _log.LogWarning("Retrying to check for question marks in folder {Folder}", fullName);
                    await DelayAsync(TimeSpan.FromSeconds(1), cancellationToken);
                }
                else
                    throw;
            }
        }

        return false;
    }

    private IAsyncFtpClient GetFtpClient(Configuration config)
    {
        IAsyncFtpClient? ftpClient = null;
        try
        {
            ftpClient = _getNewFtpClient();
            ftpClient.Host = config.HostName;
            ftpClient.Port = config.Port;
            ftpClient.Credentials = new System.Net.NetworkCredential(config.UserName, config.Password);
            return ftpClient;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error creating FTP client");
            if (ftpClient != null)
                ftpClient.Dispose();
            throw;
        }
    }

    private async Task<bool> CheckExistenceAsync(string newName, IAsyncFtpClient ftpClient, CancellationToken cancellationToken)
    {
        var file = Path.GetFileName(newName);
        var path = Path.GetDirectoryName(newName).ToLinuxPath();
        try
        {
            var files = await ftpClient.GetNameListing(path!, cancellationToken);
            return files.Contains(file);
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    private async Task DelayAsync(TimeSpan delay, CancellationToken token)
    {
        try
        {
            await Task.Delay(delay, token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task AddItemsToListAsync(string folder, List<FileItem> list, IAsyncFtpClient ftpClient, CancellationToken token)
    {
        var resultSet = await ftpClient.GetListing(folder, FtpListOption.AllFiles, token);
        foreach (var item in resultSet)
        {
            if (item.Type == FtpObjectType.Directory)
            {
                list.Add(new FileItem { Name = item.Name, FullName = item.FullName, IsFolder = true });
                await AddItemsToListAsync(item.FullName, list, ftpClient, token);
            }
            else if (item.Type == FtpObjectType.File)
            {
                list.Add(new FileItem { Name = item.Name, FullName = item.FullName, IsFolder = false });
            }
        }
    }
}