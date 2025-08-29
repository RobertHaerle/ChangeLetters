namespace ChangeLetters.IO;

public interface IDirectoryService
{
    DirectoryInfo GetDataDirectory();
}

[Export(typeof(IDirectoryService))]
public class DirectoryService(
    IConfiguration _configuration,
    ILogger<DirectoryService> _log) : IDirectoryService
{
    public DirectoryInfo GetDataDirectory()
    {
        var dataFolder = _configuration["DataDirectory"]?? Path.Combine(AppContext.BaseDirectory, "../Data");

        if (!Directory.Exists(dataFolder))
        {
            _log.LogInformation("create {folder}", dataFolder);
            Directory.CreateDirectory(dataFolder);
        }

        return new DirectoryInfo(dataFolder);
    }
}
