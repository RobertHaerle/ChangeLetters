using ChangeLetters.Domain.Ftp;

namespace ChangeLetters.Application;

public static class FtpItemExtensions
{
    internal static FileItem ToFileItem(this FtpItem ftpItem)
    {
        return new FileItem
        {
            Name = ftpItem.Name,
            FullName = ftpItem.FullName,
            IsFolder = ftpItem.IsFolder,
            FolderStatus = ConvertFolderStatus(ftpItem.FolderStatus)
        };
    }

    internal static FtpItem ToFtpItem(this FileItem fileItem)
    {
        return new FtpItem
        {
            Name = fileItem.Name,
            FullName = fileItem.FullName,
            IsFolder = fileItem.IsFolder,
            FolderStatus = ConvertFolderStatus(fileItem.FolderStatus)
        };
    }

    private static FtpFolderStatus ConvertFolderStatus(FolderStatus ftpType)
    {
        return ftpType switch
        {
            FolderStatus.Ok => FtpFolderStatus.Ok,
            FolderStatus.HasQuestionMarks => FtpFolderStatus.HasQuestionMarks,
            _ => FtpFolderStatus.Undefined
        };
    }

    private static FolderStatus ConvertFolderStatus(FtpFolderStatus fileType)
    {
        return fileType switch
        {
            FtpFolderStatus.Ok => FolderStatus.Ok,
            FtpFolderStatus.HasQuestionMarks => FolderStatus.HasQuestionMarks,
            _ => FolderStatus.Undefined
        };
    }
}