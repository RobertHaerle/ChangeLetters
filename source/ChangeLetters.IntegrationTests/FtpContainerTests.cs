using System.Net;
using ChangeLetters.IntegrationTests.Helpers;
using FluentFTP;
using Shouldly;

namespace ChangeLetters.IntegrationTests
{
    public class Tests
    {
        private IAsyncFtpClient _ftpClient;
        private CancellationTokenSource _cts;

        [SetUp]
        public void Setup()
        {
            _ftpClient = FtpHelpers.GetFtpClient();
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
            var profile = await _ftpClient.AutoConnect(_cts.Token);

            Assert.That(profile, Is.Not.Null);
        }

        [Test]
        public async Task CopyFile()
        {
            await _ftpClient.AutoConnect(_cts.Token);
            var d = await _ftpClient.GetListing(_cts.Token);
            var status = await _ftpClient.UploadFile("Files/01 - Der Ölprinz.mp3", "working/01 - Der ?lprinz.mp3", FtpRemoteExists.Overwrite, token:_cts.Token);

            status.ShouldBe(FtpStatus.Success);
        }
    }
}