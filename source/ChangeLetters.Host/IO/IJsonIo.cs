namespace ChangeLetters.IO;

/// <summary>Interface IJsonIo.</summary>
public interface IJsonIo
{
    /// <summary>Loads the specified file name.</summary>
    /// <typeparam name="T">the type to deserialize</typeparam>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>See description.</returns>
    T Load<T>(string fileName) where T : new();

    /// <summary>Saves the specified file name.</summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="data">The data.</param>
    /// <returns>See description.</returns>
    void Save<T>(string fileName, T data);
}