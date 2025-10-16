namespace ChangeLetters.Database;

/// <summary> 
/// Class DatabaseContext.
/// Inherits from <see cref="DbContext" />
/// </summary>
public class DatabaseContext : DbContext, IDatabaseContext
{
    private readonly DatabaseConfiguration _config;

    /// <summary> 
    /// Class DatabaseContext.
    /// Inherits from <see cref="DbContext" />
    /// </summary>
    public DatabaseContext(DatabaseConfiguration config) : base(config.Options)
    {
        _config = config;
    }

    /// <summary>Gets the vocabulary items.</summary>
    public DbSet<VocabularyItem> VocabularyItems => Set<VocabularyItem>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(_config.ConfigurationAssembly);
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(_config.Interceptors != null && _config.Interceptors.Any())
            optionsBuilder.AddInterceptors(_config.Interceptors!);
        base.OnConfiguring(optionsBuilder);
    }

    public void EnsureCreated()
        => Database.EnsureCreated();

    public void Migrate()
        => Database.Migrate();
}