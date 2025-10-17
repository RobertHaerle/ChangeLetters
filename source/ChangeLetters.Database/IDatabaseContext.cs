namespace ChangeLetters.Database;

public interface IDatabaseContext : IDisposable, IAsyncDisposable
{
    /// <summary>Gets the vocabulary items.</summary>
    DbSet<VocabularyItem> VocabularyItems { get; }

    void EnsureCreated();
    void Migrate();
    int SaveChanges();
    int SaveChanges(bool acceptAllChangesOnSuccess);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
}