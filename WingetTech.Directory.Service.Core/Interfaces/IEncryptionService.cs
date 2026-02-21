namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Defines encrypt/decrypt operations for sensitive configuration values.
/// </summary>
public interface IEncryptionService
{
    /// <summary>Encrypts the specified plain-text value.</summary>
    string Encrypt(string value);

    /// <summary>Decrypts the specified encrypted value.</summary>
    string Decrypt(string value);
}
