using DotNext.Threading;
using System.ComponentModel.Composition;

namespace ChangeLetters.SignalR;

/// <summary> 
/// Class ConnectionManager.
/// Implements <see cref="IConnectionManager{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
[Export(typeof(IConnectionManager<>))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class ConnectionManager<T> : IConnectionManager<T> where T : Hub
{
    private readonly HashSet<string> _connections = new();
    private readonly AsyncReaderWriterLock _rwLock = new();

    /// <inheritdoc />
    public async Task<bool> AddAsync(string connectionId)
    {
        try
        {
            await _rwLock.EnterWriteLockAsync();
            return _connections.Add(connectionId);
        }
        finally
        {
            _rwLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> RemoveAsync(string connectionId)
    {
        try
        {
            await _rwLock.EnterWriteLockAsync();
            return _connections.Remove(connectionId);
        }
        finally
        {
            _rwLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync (string? connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
            return false;

        try
        {
            await _rwLock.EnterReadLockAsync();
            return _connections.Contains(connectionId);
        }
        finally
        {
            _rwLock.Release();
        }
    }
}