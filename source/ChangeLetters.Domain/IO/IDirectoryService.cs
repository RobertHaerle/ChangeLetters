namespace ChangeLetters.Domain.IO;

/// <summary>Interface IDirectoryService.</summary>
public interface IDirectoryService
{
    /// <summary>Gets the data directory.</summary>
    /// <returns>See description.</returns>
    DirectoryInfo GetDataDirectory();
}