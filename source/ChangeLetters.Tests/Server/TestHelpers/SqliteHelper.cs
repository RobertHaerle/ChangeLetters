using ChangeLetters.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ChangeLetters.Tests.Server.TestHelpers;

public static class SqliteHelper
{
    public static DatabaseContext GetInMemoryContext(SqliteConnection connection, bool ensureCreation = true)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;
        var dbConfig = new DatabaseConfiguration
        {
            Options = options,
            ConfigurationAssembly = typeof(DatabaseContext).Assembly,
            DatabaseType = DatabaseType.Sqlite,
            ConnectionString = connection.ConnectionString
        };
        var context = new DatabaseContext(dbConfig);
        if (ensureCreation)
            context.Database.EnsureCreated();
        return context;
    }

    /// <summary>Gets a sqlite in memory connection.</summary>
    /// <remarks>Consider to dispose the connection at the end of the test. Otherwise, it will keep its data.</remarks>
    public static SqliteConnection GetSqliteInMemoryConnection()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        return connection;
    }
}
