using Microsoft.Extensions.Logging;

namespace ChangeLetters.IntegrationTests
{
    [TestFixture]
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
        public void TearDown()
        {
            _cts.Dispose();
            _ftpClient.Dispose();
        }

        [Test]
        public async Task CheckConnection()
        {
            _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test CheckConnection");
            var profile = await _ftpClient.AutoConnect(_cts.Token);

            Assert.That(profile, Is.Not.Null);
        }

        [Test]
        public async Task CopyFile()
        {
            _log.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Start test CopyFile");
            var profile = await _ftpClient.AutoConnect(_cts.Token);
            profile.ShouldNotBeNull();
            var d = await _ftpClient.GetListing(_cts.Token);
            var status = await _ftpClient.UploadFile("Files/01 - Der Ölprinz.mp3", "working/01 - Der ?lprinz.mp3", FtpRemoteExists.Overwrite, token:_cts.Token);

            status.ShouldBe(FtpStatus.Success);
        }
    }
}