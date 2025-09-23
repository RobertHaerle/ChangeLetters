using ChangeLetters.Database.Sqlite;
using Microsoft.EntityFrameworkCore.Design;

namespace ChangeLetters.Database.Manager;

/// <summary> 
/// Class DbContextFactory.
/// Implements <see cref="IDesignTimeDbContextFactory{DatabaseContext}" />
/// </summary>
/// <remarks>
/// Usage:
/// dotnet ef migrations add Initial --project ../ChangeLetters.Database.Sqlite --startup-project . -- --databaseType sqlite --connectionString "Data Source=c:/Data/ChangeLetters/ChangeLetters.db;"
/// </remarks>
public class DbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    /// <inheritdoc />
    public DatabaseContext CreateDbContext(string[] args)
    {
        var parsedArgs = ParseArgs(args);

        var databaseType = parsedArgs["databasetype"];
        var connectionString = parsedArgs["connectionstring"];

        Console.WriteLine($"found {connectionString} for {databaseType}");
        DatabaseConfiguration? databaseConfiguration = null;
        if (databaseType.ToLower() == "sqlite")
        {
            SqliteRegistration.TryGetSqliteConfiguration(connectionString, out databaseConfiguration);
        }
        else
            throw new ArgumentException("only sqlite is supported at the moment.");

        Console.WriteLine($"created database configuration\r\n{(databaseConfiguration == null ? "NULL" : databaseConfiguration.ToString())}");
        var context = new DatabaseContext(databaseConfiguration!);
        return context;
    }

    private static Dictionary<string, string> ParseArgs(string[] args)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].StartsWith("--"))
            {
                var key = args[i].Substring(2).ToLower();
                var value = args[i + 1];
                dict[key] = value;
                i++; // Skip the next argument since it's the value
            }
        }

        return dict;
    }
}
