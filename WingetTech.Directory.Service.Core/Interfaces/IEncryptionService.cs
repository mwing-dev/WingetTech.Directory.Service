namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Defines encryption and decryption operations for sensitive values.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the specified value.
    /// </summary>
    /// <param name="value">The plaintext value to encrypt.</param>
    /// <returns>The encrypted value.</returns>
    string Encrypt(string value);

    /// <summary>
    /// Decrypts the specified value.
    /// </summary>
    /// <param name="value">The encrypted value to decrypt.</param>
    /// <returns>The decrypted plaintext value.</returns>
    string Decrypt(string value);
}
