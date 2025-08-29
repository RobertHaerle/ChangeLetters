namespace ChangeLetters;

/// <summary> 
/// Class Extensions. 
/// </summary>
public static class Extensions
{
    /// <summary>Converts to a linux path.</summary>
    /// <param name="path">The path.</param>
    /// <returns>See description.</returns>
    public static string? ToLinuxPath(this string? path)
        => path?.Replace('\\', '/');
}
