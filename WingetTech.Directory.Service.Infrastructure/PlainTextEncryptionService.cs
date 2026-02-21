using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// Pass-through encryption service used as a placeholder until real encryption is implemented.
/// </summary>
/// <remarks>
/// WARNING: This implementation stores credentials as plain text.
/// It must be replaced with a real encryption implementation before production use.
/// </remarks>
public class PlainTextEncryptionService : IEncryptionService
{
    /// <inheritdoc />
    public string Encrypt(string value) => value;

    /// <inheritdoc />
    public string Decrypt(string value) => value;
}
