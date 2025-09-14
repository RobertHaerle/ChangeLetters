using FluentFTP;

namespace ChangeLetters.Tests.Server.RealWorldTests;

[Ignore("real world test")]
public class FtpClientTests
{
    private FtpClient _ftpClient;

    [SetUp]
    public void Setup()
    {
        _ftpClient = new FtpClient("192.168.0.147", "Robert", "myPassword");
    }

    [TearDown]
    public void TearDown()
    {
        _ftpClient.Dispose();
    }

    [Test]
    public void FindProfiles()
    {
        var result = _ftpClient.AutoDetect(null);
        result.Count.ShouldBe(3);
    }

    [Test]
    public void GetRoot()
    {
        _ftpClient.Connect();
        var result = _ftpClient.GetListing("/", FtpListOption.AllFiles);
        result.Count().ShouldBe(7);
    }
}