using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure.Services
{
    /// WARNING: This implementation stores credentials as plain text.
    public class PlainTextEncryptionService : IEncryptionService
    {
        public string Encrypt(string value) => value;
        public string Decrypt(string value) => value;
    }
}
