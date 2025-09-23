using ChangeLetters.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChangeLetters.Infrastructure.Sqlite.Configuration;

/// <summary> 
/// Class VocabularyConfiguration.
/// Implements <see cref="IEntityTypeConfiguration{VocabularyItem}" />
/// </summary>
public class VocabularyConfiguration : IEntityTypeConfiguration<VocabularyItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<VocabularyItem> builder)
    {
        builder.HasKey(x => x.VocabularyItemId);
        builder.Property(x=> x.VocabularyItemId)
            .IsRequired()
            .HasDefaultValueSql(SqliteSpecificSql.GetNewId());

        builder.Property(x => x.UnknownWord)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.CorrectedWord)
            .HasMaxLength(256);

        builder.HasIndex(x => x.UnknownWord).IsUnique();
    }
}
