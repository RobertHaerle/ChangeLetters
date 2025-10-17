using ChangeLetters.Shared;
using ChangeLetters.Domain;
using ChangeLetters.Application;
using ChangeLetters.Domain.Pocos;
using ChangeLetters.Domain.Ftp;

namespace ChangeLetters.Tests.Server.Interactors
{
    [TestFixture]
    public class FtpInteractorTests
    {
        private IFtpHandler _ftpHandler;
        private FtpInteractor _interactor;
        private IFtpConnector _ftpConnector;

        [SetUp]
        public void SetUp()
        {
            _ftpHandler = Substitute.For<IFtpHandler>();
            _ftpConnector = Substitute.For<IFtpConnector>();

            _interactor = new FtpInteractor(_ftpHandler, _ftpConnector);
        }

        [Test]
        public async Task RenameItemsAsync_ReturnsSucceededResult()
        {
            _ftpHandler.RenameItemsAsync(Arg.Any<string>(), Arg.Any<FileItemType>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new HandlerResult<string> { Succeeded = true }));

            var result = await _interactor.RenameItemsAsync("folder", FileItemType.File, null, CancellationToken.None);

            result.Succeeded.ShouldBeTrue();
            result.FailedFile.ShouldBeNull();
        }

        [Test]
        public async Task RenameItemsAsync_ReturnsFailedResult()
        {
            _ftpHandler.RenameItemsAsync(Arg.Any<string>(), Arg.Any<FileItemType>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new HandlerResult<string> { Succeeded = false, ErrorMessage = "error.txt" }));

            var result = await _interactor.RenameItemsAsync("folder", FileItemType.File, null, CancellationToken.None);

            result.Succeeded.ShouldBeFalse();
            result.FailedFile.ShouldBe("error.txt");
        }

        [Test]
        public async Task ReadUnknownWordsAsync_ReturnsVocabularyEntries()
        {
            var requiredVocabulary = new List<RequiredVocabulary>
            {
                new RequiredVocabulary { UnknownWord = "foo", CorrectedWord = "bar", AiResolved = false },
                new RequiredVocabulary { UnknownWord = "baz", CorrectedWord = "qux", AiResolved = true }
            };
            _ftpHandler.ReadUnknownWordsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(requiredVocabulary));

            var result = await _interactor.ReadUnknownWordsAsync("folder", CancellationToken.None);

            result.Count().ShouldBe(2);
            result.Any(e => e.UnknownWord == "foo" && e.CorrectedWord == "bar").ShouldBeTrue();
            result.Any(e => e.UnknownWord == "baz" && e.CorrectedWord == "qux").ShouldBeTrue();
        }
    }
}
