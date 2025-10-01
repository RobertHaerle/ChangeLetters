using ChangeLetters.Domain.ParseLogic;
using ChangeLetters.Shared;
using ChangeLetters.Models.Models;

namespace ChangeLetters.Tests.Server.IO;

public class FileParserTests
{
    private FileParser _subjectUnderTest;
    [SetUp]
    public void Setup()
    {
        _subjectUnderTest = new FileParser();
    }

    [Test]
    public void ReplaceUnknownWords()
    {
        var fullFileName =
            "/Backup/Karl May - Der ?lprinz (\"Ungek?rzt\")/Karl May - Der ?lprinz (\"Ungek?rzt\") CD01";
        var fileItem = new FileItem
            { FullName = fullFileName, Name = "Karl May - Der ?lprinz (\"Ungek?rzt\") CD01" };
        var vocabulary = new Dictionary<string, VocabularyItem>
        {
            {
                "?lprinz", new VocabularyItem
                {
                    UnknownWord = "?lprinz",
                    CorrectedWord = "Ölprinz"
                }
            },
            {
                "Ungek?rzt", new VocabularyItem
                {
                    UnknownWord = "Ungek?rzt",
                    CorrectedWord = "Ungekürzt"
                }
            }
        };

        var result = _subjectUnderTest.TryReplaceUnknownWordsInName(fileItem, vocabulary, out var newItem);

        Assert.That(result, Is.True);
        newItem.Name.ShouldNotContain("?");
        newItem.FullName.ShouldBe("/Backup/Karl May - Der ?lprinz (\"Ungek?rzt\")/Karl May - Der Ölprinz (\"Ungekürzt\") CD01");
    }
}
