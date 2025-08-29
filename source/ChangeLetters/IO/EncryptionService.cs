using Microsoft.AspNetCore.DataProtection;

namespace ChangeLetters.IO;

/// <summary> 
/// Class EncryptionService.
/// Implements <see cref="IEncryptionService" />
/// </summary>
[Export(typeof(IEncryptionService))]
public class EncryptionService(
    ILogger<EncryptionService> log,
    IDataProtectionProvider provider) : IEncryptionService
{
    private readonly IDataProtector _protector =
        provider.CreateProtector("ChangeLetters.EncryptionService.Password");

    public string EncryptPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;
        return _protector.Protect(password);
    }

    public string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            return string.Empty;
        try
        {
            return _protector.Unprotect(encryptedPassword);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error in password decryption.");
            return string.Empty;
        }
    }
}