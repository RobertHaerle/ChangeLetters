using FluentFTP;
using Microsoft.Extensions.Logging;

namespace ChangeLetters.IntegrationTests
{
    [TestFixture]
    [NonParallelizable]
    //[Ignore("not yet resolved network issues in github actions")]
    public class FtpContainerTests()
    {
        private CancellationTokenSource _cts;
        private ILogger<FtpContainerTests> _log;
        [SetUp]
        public void Setup()
        {
            _log = LogHelper.GetLogger<FtpContainerTests>();
            _cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public async Task TearDown()
        {
            _cts.Dispose();
         }

        [Test]
        public async Task CheckConnection()
        {
            await using var ftpClient = FtpHelpers.GetFtpClient();
            await Task.Delay(TimeSpan.FromSeconds(2));
            _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test CheckConnection");
            var profile = await ftpClient.AutoConnect(_cts.Token);

            Assert.That(profile, Is.Not.Null);
        }

        [Test]
        [Ignore("double test. This step is done in each other test")]
        public async Task CopyFile()
        {
            await using var ftpClient = FtpHelpers.GetFtpClient();
            await Task.Delay(TimeSpan.FromSeconds(2));
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
            _log.LogInformation($"upload connection found {(profile == null ? "no connection" : "connection")}");
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
            var d = await ftpClient.GetListing("/", _cts.Token);
            var result = await ftpClient.UploadFile("Files/01 - Der Ölprinz.mp3", "working/01 - Der ?lprinz.mp3", FtpRemoteExists.Overwrite, token: _cts.Token);
            await ftpClient.Disconnect();
            _log.LogInformation("upload resulted in {result}", result);
            Assert.That(result, Is.EqualTo(FtpStatus.Success));
        }
    }
}