

namespace ChangeLetters.Database.Sqlite;

public static  class SqliteRegistration
{
    /// <summary>Tries the get add sqlite configuration.</summary>
    /// <param name="config">The configuration.</param>
    /// <param name="databaseConfiguration">The database configuration.</param>
    /// <returns>True or false.</returns>
    public static bool TryGetSqliteConfiguration(IConfiguration config, out DatabaseConfiguration? databaseConfiguration)
    {
        databaseConfiguration = null;
        var connectionString = config.GetConnectionString("Sqlite");
        return TryGetSqliteConfiguration(connectionString!, out databaseConfiguration);
    }

    public static bool TryGetSqliteConfiguration(string connectionString, out DatabaseConfiguration? databaseConfiguration)
    {
        databaseConfiguration = null;
        if (string.IsNullOrEmpty(connectionString))
            return false;
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = builder.DataSource;
        var directory = Path.GetDirectoryName(dataSource);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        databaseConfiguration = new DatabaseConfiguration
        {
            DatabaseType = DatabaseType.Sqlite,
            ConnectionString = connectionString,
            Options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(connectionString, options=>
                {
                    Console.WriteLine($"MigrationAssembly: {typeof(SqliteRegistration).Assembly.GetName().Name}");
                    options.MigrationsAssembly(typeof(SqliteRegistration).Assembly.GetName().Name);
                })
                .Options,
            ConfigurationAssembly = typeof(SqliteRegistration).Assembly,
            Interceptors = [new EnableForeignKeysInterceptor()]
        };
        return true;
    }
}