using Microsoft.Extensions.Logging;

namespace ChangeLetters.IntegrationTests;

[TestFixture]
[NonParallelizable]
public class ChangeLettersTests
{
    private Configuration _config;
    private HttpClient _httpClient;
    private CancellationTokenSource _cts;
    private ILogger<ChangeLettersTests> _log;

    private const string WorkingFolder = "/api/Ftp/read-folders/%2Fworking";

    [SetUp]
    public async Task Setup()
    {
        _config = FtpHelpers.GetConfig();
        _httpClient = HttpClientHelpers.GetHttpClient();
        _log = LogHelper.GetLogger<ChangeLettersTests>();
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
        _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test HealthCheck");

        var response = await _httpClient.GetAsync("/health");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Is.EqualTo("Healthy"));
    }

    [Test]
    public async Task Connect()
    {
        _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test Connect");

        await _httpClient.GetAsync("/health");
        var response = await _httpClient.PostAsJsonAsync("/api/configuration", _config);

        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task ReadRootFolder()
    {
        _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test ReadRootFolders");

        await _httpClient.GetAsync("/health");
        var url = "/api/Ftp/read-folders/%2F";
        var response = await _httpClient.GetFromJsonAsync<FileItem[]>(url);
        response!.Length.ShouldBe(1);
        response[0].Name.ShouldBe("working");
    }

    [Test]
    public async Task ReadWorkingFolder()
    {
        _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test ReadWorkingFolder");

        await _httpClient.GetAsync("/health");
        var response = await _httpClient.GetAsync(WorkingFolder);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<FileItem[]>();
        _log.LogInformation($"result: {string.Join(",", content!.Select(c => c.Name))}");
        content!.Length.ShouldBe(0);
    }

    private async Task UploadFileAsync()
    {
        await using var ftpClient = FtpHelpers.GetFtpClient();
        _log.LogInformation("connect to FTP server {host}/{port} as {user}", ftpClient.Host, ftpClient.Port, ftpClient.Credentials.UserName);
        FtpProfile? profile = null;
        try
        {
            profile = await ftpClient.AutoConnect(_cts.Token);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "could not connect to FTP server");
        }
        _log.LogInformation($"upload connection found {(profile == null ? "no connection": "connection")}");
        if (profile == null)
        {
            try
            {
                ftpClient.Config.DataConnectionType = FtpDataConnectionType.EPSV;
                profile = await ftpClient.AutoConnect(_cts.Token);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "could not connect to FTP server with EPSV");
            }
            _log.LogInformation($"EPSV upload connection found {(profile == null ? "no connection" : "connection")}");
        }
        var d = await ftpClient.GetListing("/",_cts.Token);
        var result = await ftpClient.UploadFile("Files/01 - Der Ölprinz.mp3", "working/01 - Der ?lprinz.mp3", FtpRemoteExists.Overwrite, token: _cts.Token);
        _log.LogInformation("upload resulted in {result}", result);
    }
}