using Flurl;

namespace ChangeLetters.Client.HubConnection;

/// <summary>Interface IHubConnectionFactory.</summary>
public interface IHubConnectionFactory
{
    /// <summary>Creates a new <see cref="IHubConnection"/>.</summary>
    /// <param name="url">The URL.</param>
    /// <returns>See description.</returns>
    IHubConnection CreateConnection(Url url);
}