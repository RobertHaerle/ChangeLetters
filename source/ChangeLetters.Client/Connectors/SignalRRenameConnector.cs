using Flurl;
using ChangeLetters.Client.HubConnection;
using ChangeLetters.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ChangeLetters.Client.Connectors;

public class SignalRRenameConnector(
    IHubConnectionFactory _hubConnectionFactory,
    IWebAssemblyHostEnvironment _hostEnvironment) : ISignalRRenameConnector
{
    private IHubConnection? _hubConnection = null;

    /// <inheritdoc />
    public event Action<string>? ConnectionIdChanged;

    /// <inheritdoc />
    public event Action<CurrentItemCount>? CurrentItemCountChanged;

    /// <inheritdoc />
    public event Action<CompleteItemCount>? CompleteItemCountChanged;

    /// <inheritdoc />
    public async Task<string> ConnectAsync(CancellationToken token)
    {
        var url = new Url(_hostEnvironment.BaseAddress)
            .AppendPathSegment(SignalRPath.Rename.Path);
        _hubConnection = _hubConnectionFactory.CreateConnection(url);
        _hubConnection.Closed += HubConnectionOnClosed;
        _hubConnection.On<CurrentItemCount>(SignalRPath.Rename.CurrentItemCount, x => CurrentItemCountChanged?.Invoke(x));
        _hubConnection.On<CompleteItemCount>(SignalRPath.Rename.CompleteItemCount, x => CompleteItemCountChanged?.Invoke(x));
        await _hubConnection.StartAsync(token);
        ConnectionIdChanged?.Invoke(_hubConnection!.ConnectionId!);
        return _hubConnection.ConnectionId!;
    }

    /// <inheritdoc />
    public Task CloseAsync()
        => _hubConnection?.StopAsync() ?? Task.CompletedTask;
    
    /// <inheritdoc />
    public ValueTask DisposeAsync()
        => _hubConnection?.DisposeAsync() ?? ValueTask.CompletedTask;

    private Task HubConnectionOnClosed(Exception? ex)
    {
        ConnectionIdChanged?.Invoke(string.Empty);
        return Task.CompletedTask;
    }
}
