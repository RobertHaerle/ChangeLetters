using ChangeLetters.DTOs;
using System.Net.Http.Json;

namespace ChangeLetters.Client.Connectors;

/// <summary> 
/// Class ConfigurationConnector.
/// Implements <see cref="IConfigurationConnector" />
/// </summary>
public class ConfigurationConnector(HttpClient _httpClient) : IConfigurationConnector
{
    private const string Endpoint = "api/configuration";

    /// <inheritdoc />
    public async Task<Configuration?> GetConfigurationAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Configuration>(Endpoint);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SaveConfigurationAsync(Configuration configuration)
    {
        await _httpClient.PostAsJsonAsync(Endpoint, configuration);
    }
}
