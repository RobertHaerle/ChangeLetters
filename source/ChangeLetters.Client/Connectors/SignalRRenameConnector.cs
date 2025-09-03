using Flurl;
using ChangeLetters.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ChangeLetters.Client.Connectors;

public class SignalRRenameConnector(IWebAssemblyHostEnvironment _hostEnvironment) : ISignalRRenameConnector
{
    private HubConnection? _hubConnection = null;

    /// inheritdoc />
    public event Action<string>? ConnectionIdChanged;

    /// inheritdoc />
    public event Action<CurrentItemCount>? CurrentItemCountChanged;

    /// inheritdoc />
    public event Action<CompleteItemCount>? CompleteItemCountChanged;

    /// inheritdoc />
    public async Task<string> ConnectAsync(CancellationToken token)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hostEnvironment.BaseAddress
                .AppendPathSegment(SignalRPath.Rename.Path))
            .Build();
        _hubConnection.Closed += HubConnectionOnClosed;
        _hubConnection.On<CurrentItemCount>(SignalRPath.Rename.CurrentItemCount, x => CurrentItemCountChanged?.Invoke(x));
        _hubConnection.On<CompleteItemCount>(SignalRPath.Rename.CompleteItemCount, x => CompleteItemCountChanged?.Invoke(x));
        await _hubConnection.StartAsync(token);
        ConnectionIdChanged?.Invoke(_hubConnection!.ConnectionId!);
        return _hubConnection.ConnectionId!;
    }

    /// inheritdoc />
    public Task CloseAsync()
        => _hubConnection?.StopAsync() ?? Task.CompletedTask;
    
    /// inheritdoc />
    public ValueTask DisposeAsync()
        => _hubConnection?.DisposeAsync() ?? ValueTask.CompletedTask;

    private Task HubConnectionOnClosed(Exception? ex)
    {
        ConnectionIdChanged?.Invoke(string.Empty);
        return Task.CompletedTask;
    }

}
