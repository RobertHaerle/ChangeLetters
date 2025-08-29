using System.Net;
using System.Text;
using ChangeLetters.DTOs;
using ChangeLetters.Client.Connectors;
using ChangeLetters.Tests.Client.ClientHelpers;

namespace ChangeLetters.Tests.Client.Connectors;

public class FtpConnectorClientTests
{
    [Test]
    public async Task ConnectAsync_ReturnsTrue_OnSuccess()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new FtpConnectorClient(httpClient);

        // Act
        var result = await client.ConnectAsync(new Configuration());

        // Assert
        result.ShouldBe(true);
    }

    [Test]
    public async Task ConnectAsync_SendsRequestToCorrectUri()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var client = new FtpConnectorClient(httpClient);

        // Act
        await client.ConnectAsync(new Configuration());

        // Assert
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/ftp/connect");
    }

    [Test]
    public async Task ReadFoldersAsync_ReturnsFolders()
    {
        var fi = GetFileItem();
        fi.Name = "Folder1";
        fi.IsFolder = true;
        var folders = new[] { fi };
        var json = System.Text.Json.JsonSerializer.Serialize(folders);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var client = new FtpConnectorClient(httpClient);

        var result = await client.ReadFoldersAsync("test", CancellationToken.None);

        result.ShouldNotBeNull();
        result.Length.ShouldBe(1);
        result[0].Name.ShouldBe("Folder1");
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/ftp/read-folders/test");
    }

    [Test]
    public async Task CheckQuestionMarksAsync_SendsRequestToCorrectUri()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var client = new FtpConnectorClient(httpClient);

        await client.CheckQuestionMarksAsync(GetFileItem(), CancellationToken.None);

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/ftp/check-question-marks");
    }

    [Test]
    public async Task RenameItemsAsync_SendsRequestAndReturnsResult()
    {
        // Arrange
        var expectedResult = new RenameFileItemsResult { Succeeded = true };
        var json = System.Text.Json.JsonSerializer.Serialize(expectedResult);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var client = new FtpConnectorClient(httpClient);
        var request = new RenameFileItemsRequest { Folder = "folder", FileItemType = 0 };

        // Act
        var result = await client.RenameItemsAsync(request, CancellationToken.None);

        // Assert
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("http://localhost/api/ftp/rename-items");
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    private FileItem GetFileItem()
    => new() { FullName = "FullName", Name = "file.txt", IsFolder = false };
}
