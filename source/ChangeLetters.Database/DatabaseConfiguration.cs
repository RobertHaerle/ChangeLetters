using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ChangeLetters.Database;

/// <summary> 
/// Class DatabaseConfiguration. 
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>Gets or sets the configuration assembly.</summary>
    /// <remarks>
    /// Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" />
    ///     instances that are defined in provided assembly.
    /// <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/>
    /// </remarks>
    public required Assembly ConfigurationAssembly { get; init; }

    /// <summary>Gets or sets the DB context options.</summary>
    public required DbContextOptions<DatabaseContext> Options { get; init; }

    /// <summary>Gets or sets the type of the database.</summary>
    public required DatabaseType DatabaseType { get; init; }

    /// <summary>Gets or sets the connection string.</summary>
    public required string ConnectionString { get; init; }

    public IInterceptor[]? Interceptors { get; init; }

    public override string ToString()
    {
        return $"\r\nConfigurationAssembly: {ConfigurationAssembly.FullName}\r\n"
            + $"DatabaseType: {DatabaseType}\r\n"
                + $"ConnectionString: {ConnectionString}\r\n"
                + $"Options: {Options}\r\n"
                + $"Interceptors: {(Interceptors != null ? string.Join(", ", Interceptors.Select(i => i.GetType().Name)) : "None")}";
    }
}


