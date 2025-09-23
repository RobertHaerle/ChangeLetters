using ChangeLetters.Infrastructure;
using ChangeLetters.Infrastructure.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ChangeLetters.StartUp;

/// <summary> 
/// Class DatabaseRegistration. 
/// </summary>
internal static class DatabaseRegistration
{
    /// <summary>Adds the database.</summary>
    /// <param name="services">The services.</param>
    /// <param name="config">The configuration.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">No valid database configuration found.</exception>
    internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        if (SqliteRegistration.TryGetSqliteConfiguration(config, out var dbConfig)
            || TryGetSqlServerConfiguration(config, out  dbConfig))
        {
            services.AddSingleton(dbConfig!);
            services.AddDbContext<DatabaseContext>();
        }
        else
            throw new InvalidOperationException("No valid database configuration found.");
        return services;
    }

    private static bool TryGetSqlServerConfiguration(IConfiguration config, out DatabaseConfiguration? databaseConfiguration)
    {
        var connectionString = config.GetConnectionString("SqlServer");
        databaseConfiguration = null;
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

    /// <summary>Initializes the database.</summary>
    /// <param name="app">The application.</param>
    /// <returns><see cref="WebApplication"/>.</returns>
    internal static WebApplication InitializeDatabase(this WebApplication app)
    {
        var config = app.Services.GetRequiredService<DatabaseConfiguration>();
        var context = app.Services.GetRequiredService<DatabaseContext>();
        context.Database.EnsureCreated();
        if (config.DatabaseType != DatabaseType.Sqlite)
            context.Database.Migrate();
        return app; 
    }
}
