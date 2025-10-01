using System.Net;
using System.Text;
using System.Text.Json;
using ChangeLetters.Client.Connectors;
using ChangeLetters.Shared;
using ChangeLetters.Tests.Client.ClientHelpers;

namespace ChangeLetters.Tests.Client.Connectors;

public class ConfigurationConnectorTests
{
    [Test]
    public async Task GetConfigurationAsync_ReturnsConfiguration()
    {
        var config = new Configuration { HostName = "host", Port = 21, UserName = "user", Password = "pw" };
        var json = JsonSerializer.Serialize(config);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var connector = new ConfigurationConnector(httpClient);

        var result = await connector.GetConfigurationAsync();

        result.ShouldNotBeNull();
        result!.HostName.ShouldBe("host");
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/configuration");
    }

    [Test]
    public async Task SaveConfigurationAsync_SendsRequestToCorrectUri()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var connector = new ConfigurationConnector(httpClient);

        await connector.SaveConfigurationAsync(new Configuration());

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/configuration");
        handler.LastRequest.Method.Method.ShouldBe("POST");
    }
}
