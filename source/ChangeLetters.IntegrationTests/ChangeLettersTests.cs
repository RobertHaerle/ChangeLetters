

namespace ChangeLetters.IntegrationTests;

public class ChangeLettersTests
{
    private Configuration _config;
    private HttpClient _httpClient;
    private CancellationTokenSource _cts;
    private const string WorkingFolder = "/api/Ftp/read-folders/%2F";
    [SetUp]
    public async Task Setup()
    {
        _config = new Configuration
        {
            HostName = "172.23.0.10",
            Port = 21,
            UserName = "myuser",
            Password = "mypass",
        };

        _httpClient = HttpClientHelpers.GetHttpClient();
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await UploadFileAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _cts.Dispose();
        _httpClient.Dispose();
    }

    [Test]
    public async Task HealthCheck()
    {
        var response = await _httpClient.GetAsync("/health");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Is.EqualTo("Healthy"));
    }

    [Test]
    public async Task Connect()
    {

        var response = await _httpClient.PostAsJsonAsync("/api/configuration", _config);

        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task ReadRootFolder()
    {
        var url = "/api/Ftp/read-folders/%2F";
        var response = await _httpClient.GetFromJsonAsync<FileItem[]>(url);
        response!.Length.ShouldBe(1);
        response[0].Name.ShouldBe("working");
    }

    [Test]
    public async Task ReadWorkingFolder()
    {
        var response = await _httpClient.GetAsync(WorkingFolder);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<FileItem[]>();
        content!.Length.ShouldBe(0);
    }


    private async Task UploadFileAsync()
    {
        await using var ftpClient = FtpHelpers.GetFtpClient();
        await ftpClient.AutoConnect(_cts.Token);
        var d = await ftpClient.GetListing(_cts.Token);
        var result = await ftpClient.UploadFile("Files/01 - Der Ölprinz.mp3", "working/01 - Der ?lprinz.mp3", FtpRemoteExists.Overwrite, token: _cts.Token);
    }

}
