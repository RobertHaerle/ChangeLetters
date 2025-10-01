using System.Text.Json;

namespace ChangeLetters.IO;

/// <summary> 
/// Class JsonIo.
/// Implements <see cref="IJsonIo" />
/// </summary>
[Export(typeof(IJsonIo))]
public class JsonIo(
    ILogger<JsonIo> _log,
    IDirectoryService _directoryService) : IJsonIo
{
    /// <inheritdoc />
    public T Load<T>(string fileName) where T : new()
    {
        try
        {
            var dir = _directoryService.GetDataDirectory();
            var filePath = Path.Combine(dir.FullName, fileName);
            if (!File.Exists(filePath))
            {
                _log.LogWarning("File {file} does not exist", filePath);
                return new T();
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "");
        }

        return new T();
    }

    /// <inheritdoc />
    public void Save<T>(string fileName, T data)
    {
        try
        {
            var dir = _directoryService.GetDataDirectory();
            var filePath = Path.Combine(dir.FullName, fileName);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            _log.LogInformation("Saved data to {file}", filePath);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "error saving json data");
        }
    }
}