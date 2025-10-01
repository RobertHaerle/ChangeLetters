using System.Net;
using System.Text;
using System.Text.Json;
using ChangeLetters.Client.Connectors;
using ChangeLetters.Shared;
using ChangeLetters.Tests.Client.ClientHelpers;

namespace ChangeLetters.Tests.Client.Connectors;

public class VocabularyConnectorTests
{
    [Test]
    public async Task UpsertEntriesAsync_SendsRequestToCorrectUri()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var connector = new VocabularyConnector(httpClient);

        await connector.UpsertEntriesAsync(new List<VocabularyEntry>());

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/Vocabulary/Upsert");
        handler.LastRequest.Method.Method.ShouldBe("PUT");
    }

    [Test]
    public async Task RebuildAllItemsAsync_SendsRequestToCorrectUri()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var connector = new VocabularyConnector(httpClient);

        await connector.RebuildAllItemsAsync(new List<VocabularyEntry>());

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/Vocabulary/RebuildAll");
        handler.LastRequest.Method.Method.ShouldBe("POST");
    }

    [Test]
    public async Task GetAllItemsAsync_ReturnsVocabularyEntries()
    {
        var entries = new List<VocabularyEntry> { new VocabularyEntry { UnknownWord = "foo", CorrectedWord = "bar" } };
        var json = JsonSerializer.Serialize(entries);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var connector = new VocabularyConnector(httpClient);

        var result = await connector.GetAllItemsAsync();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result["foo"].UnknownWord.ShouldBe("foo");
        result["foo"].CorrectedWord.ShouldBe("bar");
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/Vocabulary");
        handler.LastRequest.Method.Method.ShouldBe("GET");
    }
}
