namespace ChangeLetters.Extensions;

/// <summary> 
/// Class StringExtensions. 
/// </summary>
public static class StringExtensions
{
    /// <summary>Converts to a linux path.</summary>
    /// <param name="path">The path.</param>
    /// <returns>See description.</returns>
    public static string? ToLinuxPath(this string? path)
        => path?.Replace('\\', '/');

    /// <summary>Determines whether this string is null or empty.</summary>
    /// <param name="value">The value.</param>
    /// <returns>True or false.</returns>
    public static bool IsNullOrEmpty(this string? value)
        => string.IsNullOrEmpty(value);

}
