using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure.Services;

/// <summary>
/// Pass-through encryption service that stores values as plaintext.
/// This is a temporary implementation; real encryption will be added later.
/// </summary>
public class PlainTextEncryptionService : IEncryptionService
{
    /// <inheritdoc />
    public string Encrypt(string value) => value;

    /// <inheritdoc />
    public string Decrypt(string value) => value;
}
