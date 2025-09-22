using FluentFTP;
using Microsoft.Extensions.Logging;

namespace ChangeLetters.IntegrationTests
{
    [TestFixture]
    [NonParallelizable]
    //[Ignore("not yet resolved network issues in github actions")]
    public class FtpContainerTests()
    {
        private IAsyncFtpClient _ftpClient;
        private CancellationTokenSource _cts;
        private ILogger<FtpContainerTests> _log;
        [SetUp]
        public void Setup()
        {
            _ftpClient = FtpHelpers.GetFtpClient();
            _log = LogHelper.GetLogger<FtpContainerTests>();
            _cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public async Task TearDown()
        {
            _cts.Dispose();
            await _ftpClient.Disconnect();
            _ftpClient.Dispose();
        }

        [Test]
        public async Task CheckConnection()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test CheckConnection");
            var profile = await _ftpClient.AutoConnect(_cts.Token);

            Assert.That(profile, Is.Not.Null);
        }

        [Test]
        public async Task CopyFile()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test CopyFile");
            FtpProfile? profile = null;
            try
            {
                profile = await _ftpClient.AutoConnect(_cts.Token);
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
                    _ftpClient.Config.DataConnectionType = FtpDataConnectionType.EPSV;
                    profile = await _ftpClient.AutoConnect(_cts.Token);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "could not connect to FTP server with EPSV");
                }
                _log.LogInformation($"EPSV upload connection found {(profile == null ? "no connection" : "connection")}");
            }
            var d = await _ftpClient.GetListing("/", _cts.Token);
            var status = await _ftpClient.UploadFile("Files/01 - Der Ölprinz.mp3", "working/01 - Der ?lprinz.mp3", FtpRemoteExists.Overwrite, token:_cts.Token);

            status.ShouldBe(FtpStatus.Success);
        }
    }
}