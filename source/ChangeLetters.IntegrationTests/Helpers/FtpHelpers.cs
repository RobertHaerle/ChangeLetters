namespace ChangeLetters.IntegrationTests.Helpers;

public static class FtpHelpers
{
    public static IAsyncFtpClient GetFtpClient()
    {
        var config = GetConfig();
        var ftpClient = new AsyncFtpClient();
        ftpClient.Host = "127.0.0.1";
        ftpClient.Port = config.Port;
        ftpClient.Credentials = new NetworkCredential(config.UserName, config.Password);

        ftpClient.Config.DataConnectionType = FtpDataConnectionType.EPSV;
        ftpClient.Config.ConnectTimeout = 5000;
        ftpClient.Config.ReadTimeout = 5000;

        return ftpClient;
    }

    public static Configuration GetConfig()
    => new ()
    {
        HostName = "host.docker.internal",
        Port = 2121,
        UserName = "myuser",
        Password = "mypass",
    };
}
