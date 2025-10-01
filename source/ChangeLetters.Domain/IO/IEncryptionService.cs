namespace ChangeLetters.Domain.IO;

/// <summary>Interface IEncryptionService.</summary>
public interface IEncryptionService
{
    /// <summary>Encrypts the password.</summary>
    /// <param name="password">The password.</param>
    /// <returns>See description.</returns>
    string EncryptPassword(string password);

    /// <summary>Decrypts the password.</summary>
    /// <param name="encryptedPassword">The encrypted password.</param>
    /// <returns>See description.</returns>
    string DecryptPassword(string encryptedPassword);
}
