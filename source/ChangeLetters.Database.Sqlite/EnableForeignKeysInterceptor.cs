using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ChangeLetters.Database.Sqlite;

/// <summary>
/// Interceptor that ensures SQLite foreign key constraints are enabled for every database connection.
/// This class executes the <c>PRAGMA foreign_keys = ON;</c> command whenever a connection is opened,
/// guaranteeing referential integrity enforcement in SQLite databases used by Entity Framework Core.
/// </summary>
public class EnableForeignKeysInterceptor : DbConnectionInterceptor
{
    /// <inheritdoc />
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (connection is SqliteConnection sqliteConnection)
        {
            using var command = sqliteConnection.CreateCommand();
            command.CommandText = SqliteSpecificSql.EnableForeignKeys;
            command.ExecuteNonQuery();
        }
        base.ConnectionOpened(connection, eventData);
    }

    /// <inheritdoc />
    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = new CancellationToken())
    {
        if (connection is SqliteConnection sqliteConnection)
        {
            await using var command = sqliteConnection.CreateCommand();
            command.CommandText = SqliteSpecificSql.EnableForeignKeys;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}