using ChangeLetters.Infrastructure;
using ChangeLetters.Infrastructure.Sqlite;
using Microsoft.EntityFrameworkCore.Design;

namespace ChangeLetters.Database.Manager;

/// <summary> 
/// Class DbContextFactory.
/// Implements <see cref="IDesignTimeDbContextFactory{DatabaseContext}" />
/// </summary>
public class DbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    /// <inheritdoc />
    public DatabaseContext CreateDbContext(string[] args)
    {
        if (args.Length != 2)
            throw new ArgumentException("Expected exactly two arguments:\r\n 1. database type \r\n 2. connection string.");
        var databaseType = args[0];
        var connectionString = args[1];

        DatabaseConfiguration? options = null;

        if (databaseType.ToLower() == "sqlite")
            SqliteRegistration.TryGetSqliteConfiguration(connectionString, out var configuration);
        else
            throw new ArgumentException("only sqlite is supported at the moment.");

        return new DatabaseContext(options!);
    }
}
