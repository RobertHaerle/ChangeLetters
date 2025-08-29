using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
    public required Assembly ConfigurationAssembly { get; set; }

    /// <summary>Gets or sets the DB context options.</summary>
    public required DbContextOptions<DatabaseContext> Options { get; set; }
}
