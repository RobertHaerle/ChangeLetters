using Flurl;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChangeLetters.Client.HubConnection;

/// <summary> 
/// Class HubConnectionFactory.
/// Implements <see cref="IHubConnectionFactory" />
/// </summary>
public class HubConnectionFactory : IHubConnectionFactory
{
    /// <inheritdoc />
    public IHubConnection CreateConnection(Url url)
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
        return new HubConnectionWrapper(hubConnection);
    }
}