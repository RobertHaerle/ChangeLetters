using Flurl;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ChangeLetters.Client.Connectors;

public class SignalRRenameConnector(IWebAssemblyHostEnvironment _hostEnvironment) : IAsyncDisposable
{
    private HubConnection? _hubConnection = null;
    public event Action<string> ConnectionIdChanged;
    private async Task AddSignalRAsync(string path, string methodName)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hostEnvironment.BaseAddress
                .AppendPathSegment(path))
            .Build();
        //_hubConnection.On<object>(methodName, x => Console.WriteLine(x));
        //_hubConnection.On<T>(methodName, x => SignalReceived.Raise(x, methodName));
        await _hubConnection.StartAsync();
        ConnectionIdChanged?.Invoke(_hubConnection!.ConnectionId!);
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
