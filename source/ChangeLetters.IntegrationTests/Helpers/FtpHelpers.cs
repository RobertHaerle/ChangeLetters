using FluentFTP;

namespace ChangeLetters.IntegrationTests.Helpers;

public static class FtpHelpers
{
    public static IAsyncFtpClient GetFtpClient()
    {
        IAsyncFtpClient? ftpClient = null;
        ftpClient = new AsyncFtpClient();
        ftpClient.Host = "127.0.0.1";
        ftpClient.Port = 2121;
        ftpClient.Credentials = new System.Net.NetworkCredential("myuser", "mypass");
        return ftpClient;
    }
}
