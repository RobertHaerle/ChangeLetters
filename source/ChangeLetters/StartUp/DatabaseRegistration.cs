using Microsoft.Data.Sqlite;
using ChangeLetters.Database;
using Microsoft.EntityFrameworkCore;

namespace ChangeLetters.StartUp;

internal static class DatabaseRegistration
{
    internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        if (AddSqliteContext(services, config) || AddSqlServerContext(services, config))
            services.AddDbContext<DatabaseContext>();
        else
            throw new InvalidOperationException("No valid database configuration found.");
        return services;
    }

    private static bool AddSqlServerContext(IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SqlServer");
        if (string.IsNullOrEmpty(connectionString))
            return false;

        // to use SQL Server, uncomment the following line and ensure you have the necessary package
        // consider creating Database migrations and pack the different migrations into a different assembly.

        //var dbConfig = new DatabaseConfiguration
        //{
        //    Options = new DbContextOptionsBuilder<DatabaseContext>()
        //        .UseSqlServer(connectionString)
        //        .Options,
        //    ConfigurationAssembly = typeof(SomeClass).Assembly
        //};
        //services.AddSingleton(dbConfig);
        return false;
    }

    private static bool AddSqliteContext(IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Sqlite");
        if (string.IsNullOrEmpty(connectionString))
            return false;
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = builder.DataSource;
        var directory = Path.GetDirectoryName(dataSource);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        var dbConfig = new DatabaseConfiguration
        {
            Options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(connectionString)
                .Options,
            ConfigurationAssembly = typeof(DatabaseContext).Assembly
        };
        services.AddSingleton(dbConfig);
        return true;
    }
}
