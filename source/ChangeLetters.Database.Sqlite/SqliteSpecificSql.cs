namespace ChangeLetters.Infrastructure.Sqlite;

internal static class SqliteSpecificSql
{
    public const string EnableForeignKeys = "PRAGMA foreign_keys = ON;";

    /// <summary>Gets the new identifier.</summary>
    /// <returns>See description.</returns>
    public static string GetNewId()
        => "(lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('AB89',abs(random()) % 4 + 1,1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))))";
}
