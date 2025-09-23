using ChangeLetters.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ChangeLetters.Infrastructure;

/// <summary> 
/// Class DatabaseContext.
/// Inherits from <see cref="DbContext" />
/// </summary>
public class DatabaseContext(DatabaseConfiguration config) : DbContext(config.Options)
{
    /// <summary>Gets the vocabulary items.</summary>
    public DbSet<VocabularyItem> VocabularyItems => Set<VocabularyItem>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(config.ConfigurationAssembly);
    }
}