namespace ChangeLetters.Database;

/// <summary> 
/// Class DatabaseContext.
/// Inherits from <see cref="DbContext" />
/// </summary>
public class DatabaseContext : DbContext
{
    private readonly DatabaseConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
    /// </summary>
    /// <remarks>For test purposes only. Better way would be an interface!!!.</remarks>
    internal DatabaseContext() : base()
    {
    }

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
}