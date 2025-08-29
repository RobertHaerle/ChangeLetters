using ChangeLetters.DTOs;
using ChangeLetters.Model;
using ChangeLetters.Models;
using ChangeLetters.ParseLogic;

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
            "/Download/[Sci-Fi] Brandon Q. Morris - M?bius (3) - Das zeitlose Artefakt (\"Ungek?rzt\") - filecrypt.cc/Brandon Q. Morris - M?bius (3) - Das zeitlose Artefakt (Ungek?rzt)";
        var fileItem = new FileItem
            { FullName = fullFileName, Name = "Brandon Q. Morris - M?bius (3) - Das zeitlose Artefakt (Ungek?rzt)" };
        var vocabulary = new Dictionary<string, VocabularyItem>
        {
            {
                "M?bius", new VocabularyItem
                {
                    UnknownWord = "M?bius",
                    CorrectedWord = "Möbius"
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
        newItem.FullName.ShouldBe("/Download/[Sci-Fi] Brandon Q. Morris - M?bius (3) - Das zeitlose Artefakt (\"Ungek?rzt\") - filecrypt.cc/Brandon Q. Morris - Möbius (3) - Das zeitlose Artefakt (Ungekürzt)");
    }
}
