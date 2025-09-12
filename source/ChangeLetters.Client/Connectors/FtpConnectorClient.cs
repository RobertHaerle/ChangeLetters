using ChangeLetters.DTOs;
using System.Net.Http.Json;

namespace ChangeLetters.Client.Connectors;

/// <summary> 
/// Class FtpConnectorClient.
/// Implements <see cref="IFtpConnectorClient" />
/// </summary>
public class FtpConnectorClient : IFtpConnectorClient
{
    private readonly HttpClient _http;
    public FtpConnectorClient(HttpClient http)
    {
        _http = http;
    }

    /// <inheritdoc />
    public async Task<bool> ConnectAsync(Configuration config)
    {
        var response = await _http.PostAsJsonAsync("api/ftp/connect", config);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task<FileItem[]> ReadFoldersAsync(string folder, CancellationToken token)
    {
        try
        {
            var response = await _http.GetAsync($"api/ftp/read-folders/{Uri.EscapeDataString(folder)}", token);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<FileItem[]>(token))!;
        }
        catch (TaskCanceledException)
        {
            return [];
        }
    }

    /// <inheritdoc />
    public async Task CheckQuestionMarksAsync(FileItem fileItem, CancellationToken token)
    {
        try
        {
            var response = await _http.PutAsJsonAsync("api/ftp/check-question-marks", fileItem, token);
            response.EnsureSuccessStatusCode();
            var folderStatus = await response.Content.ReadFromJsonAsync<FolderStatus>(token);
            fileItem.FolderStatus = folderStatus;
        }
        catch (TaskCanceledException)
        {
            // No action needed if cancelled
        }
    }

    public async Task<Dictionary<string, VocabularyEntry>> ReadUnknownWordsAsync(string folder, CancellationToken token)
    {
        try
        {
            var response = await _http.GetAsync($"api/ftp/read-unknown-words/{Uri.EscapeDataString(folder)}", token);
            if (response.IsSuccessStatusCode)
            {
                var entries = await response.Content.ReadFromJsonAsync<List<VocabularyEntry>>(token);
                return entries!.ToDictionary(x => x.UnknownWord);
            }
        }
        catch (TaskCanceledException)
        {
        }

        return new();
    }

    /// <inheritdoc />
    public async Task<RenameFileItemsResult> RenameItemsAsync(RenameFileItemsRequest request, CancellationToken token)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/ftp/rename-items", request, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<RenameFileItemsResult>(token);
            return result!;
        }
        catch (TaskCanceledException)
        {
            return new RenameFileItemsResult
            {
                FailedFile = "action aborted.",
                Succeeded = false,
            };
        }
    }
}
