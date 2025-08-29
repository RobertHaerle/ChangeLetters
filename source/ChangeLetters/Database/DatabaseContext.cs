using ChangeLetters.Models;
using Microsoft.EntityFrameworkCore;

namespace ChangeLetters.Database;

public class DatabaseContext(DatabaseConfiguration config) : DbContext(config.Options)
{
    public DbSet<VocabularyItem> VocabularyItems => Set<VocabularyItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(config.ConfigurationAssembly);
    }
}