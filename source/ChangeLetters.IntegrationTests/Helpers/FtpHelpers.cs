namespace ChangeLetters.IntegrationTests.Helpers;

public static class FtpHelpers
{
    public static IAsyncFtpClient GetFtpClient()
    {
        var ftpClient = new AsyncFtpClient();
        ftpClient.Host = "127.0.0.1";
        ftpClient.Port = 2121;
        ftpClient.Credentials = new NetworkCredential("myuser", "mypass");
        return ftpClient;
    }
}
