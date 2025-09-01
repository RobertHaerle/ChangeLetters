namespace ChangeLetters.SignalR;

/// <summary>Interface IConnectionManager.</summary>
/// <typeparam name="T"></typeparam>
public interface IConnectionManager<T> where T : Hub
{
    /// <summary>Add as an asynchronous operation.</summary>
    /// <param name="connectionId">The connection identifier.</param>
    /// <returns>true if the operation succeeded.</returns>
    Task<bool> AddAsync(string connectionId);

    /// <summary>Remove as an asynchronous operation.</summary>
    /// <param name="connectionId">The connection identifier.</param>
    /// <returns>true if the operation succeeded.</returns>
    Task<bool> RemoveAsync(string connectionId);

    /// <summary>Exists as an asynchronous operation.</summary>
    /// <param name="connectionId">The connection identifier.</param>
    /// <returns>true if the connection exists.</returns>
    Task<bool> ExistsAsync (string? connectionId);
}