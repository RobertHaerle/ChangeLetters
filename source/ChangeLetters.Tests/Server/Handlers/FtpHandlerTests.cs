using ChangeLetters.IO;
using ChangeLetters.Shared;
using ChangeLetters.Handlers;
using ChangeLetters.ParseLogic;
using ChangeLetters.Connectors;
using ChangeLetters.Repositories;
using ChangeLetters.Models.Models;

namespace ChangeLetters.Tests.Server.Handlers
{
    public class FtpHandlerTests
    {
        private FtpHandler _sut;
        private IFileParser _fileParser;
        private ILogger<FtpHandler> _log;
        private List<FileItem> _fileItems;
        private IFtpConnector _ftpConnector;
        private Configuration _configuration;
        private IConfigurationIo _configurationIo;
        private IVocabularyHandler _vocabularyHandler;
        private IVocabularyRepository _vocabularyRepository;
        private ISignalRRenameHandler _signalRRenameHandler;
        [SetUp]
        public void Setup()
        {
            _configuration = new Configuration();
            _fileParser = Substitute.For<IFileParser>();
            _log = Substitute.For<ILogger<FtpHandler>>();
            _ftpConnector = Substitute.For<IFtpConnector>();
            _configurationIo = Substitute.For<IConfigurationIo>();
            _vocabularyHandler = Substitute.For<IVocabularyHandler>();
            _vocabularyRepository = Substitute.For<IVocabularyRepository>();
            _signalRRenameHandler = Substitute.For<ISignalRRenameHandler>();
            _fileItems = [new() { Name = "file?1.txt", FullName = "/folder/file?1.txt", IsFolder = false }];

            _configurationIo.GetConfiguration().Returns(_configuration);

            _sut = new FtpHandler(
                _fileParser,
                _log,
                _ftpConnector,
                _configurationIo,
                _vocabularyHandler,
                _vocabularyRepository,
                _signalRRenameHandler);
        }

        [Test]
        public async Task RenameItemsAsync_Succeeds_WhenAllItemsRenamed()
        {
            // Arrange
            var vocabularyItems = new[]
            {
                new VocabularyItem { UnknownWord = "?", CorrectedWord = "a" }
            };
            _ftpConnector.ReadFilesAsync(_configuration, Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(_fileItems);
            _vocabularyRepository.GetAllItemsAsync(Arg.Any<CancellationToken>())
                .Returns(vocabularyItems);
            _fileParser.TryReplaceUnknownWordsInName(
                Arg.Any<FileItem>(),
                Arg.Any<Dictionary<string, VocabularyItem>>(),
                out Arg.Any<FileItem>())
                .Returns(x =>
                {
                    x[2] = new FileItem { Name = "filea1.txt", FullName = "/folder/filea1.txt", IsFolder = false };
                    return true;
                });
            _ftpConnector.RenameFileAsync(
                Arg.Any<FileItem>(), Arg.Any<string>(), _configuration, Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _sut.RenameItemsAsync("/folder", FileItemType.File, "ci", CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeTrue();
            result.FailedFile.ShouldBeNull();
        }

        [Test]
        public async Task RenameItemsAsync_Fails_WhenRenameFails()
        {
            // Arrange
            var vocabularyItems = new[]
            {
                new VocabularyItem { UnknownWord = "?", CorrectedWord = "a" }
            };
            _ftpConnector.ReadFilesAsync(_configuration, Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(_fileItems);
            _vocabularyRepository.GetAllItemsAsync(Arg.Any<CancellationToken>())
                .Returns(vocabularyItems);
            _fileParser.TryReplaceUnknownWordsInName(
                Arg.Any<FileItem>(),
                Arg.Any<Dictionary<string, VocabularyItem>>(),
                out Arg.Any<FileItem>())
                .Returns(x =>
                {
                    x[2] = new FileItem { Name = "filea1.txt", FullName = "/folder/filea1.txt", IsFolder = false };
                    return true;
                });
            _ftpConnector.RenameFileAsync(
                Arg.Any<FileItem>(), Arg.Any<string>(), _configuration, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _sut.RenameItemsAsync("/folder", FileItemType.File, "ci", CancellationToken.None);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.FailedFile.ShouldBe("/folder/file?1.txt");
        }

        [Test]
        public async Task ReadUnknownWordsAsync_ReturnsVocabulary_WhenUnknownWordsFound()
        {
            // Arrange
            var unknownWords = new List<string> { "?" };
            var vocabularyEntries = new List<VocabularyEntry>
            {
                new VocabularyEntry { UnknownWord = "?", CorrectedWord = "a" }
            };
            _ftpConnector.ReadFilesAsync(_configuration, Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(_fileItems);
            _fileParser.GetWordsWithUnknownLetters(_fileItems).Returns(unknownWords);
            _vocabularyHandler.GetRequiredVocabularyAsync(unknownWords, Arg.Any<CancellationToken>())
                .Returns(vocabularyEntries);

            // Act
            var result = await _sut.ReadUnknownWordsAsync("/folder", CancellationToken.None);

            // Assert
            result.Count.ShouldBe(1);
            result[0].UnknownWord.ShouldBe("?");
            result[0].CorrectedWord.ShouldBe("a");
        }

        [Test]
        public async Task ReadUnknownWordsAsync_ReturnsEmpty_WhenNoUnknownWords()
        {
            // Arrange
            _fileItems[0].FullName = "/folder/file1.txt";
            _ftpConnector.ReadFilesAsync(_configuration, Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(_fileItems);
            _fileParser.GetWordsWithUnknownLetters(_fileItems).Returns(new List<string>());

            // Act
            var result = await _sut.ReadUnknownWordsAsync("/folder", CancellationToken.None);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
