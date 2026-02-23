using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure.Services
{
    public class DataProtectionEncryptionService : IEncryptionService
    {
        private readonly IDataProtector _protector;
        private readonly ILogger<DataProtectionEncryptionService> _logger;

        public DataProtectionEncryptionService(IDataProtectionProvider provider, ILogger<DataProtectionEncryptionService> logger)
        {
            _protector = provider.CreateProtector("DirectorySettings.BindPassword");
            _logger = logger;
        }

        public string Encrypt(string value) => _protector.Protect(value);

        public string Decrypt(string value)
        {
            try
            {
                return _protector.Unprotect(value);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                // Value may have been stored before encryption was enabled; return as-is.
                // Log at warning level so administrators can re-save settings to encrypt the value.
                _logger.LogWarning("Decryption failed for a stored value; it may be unencrypted. Re-save directory settings to encrypt it.");
                return value;
            }
        }
    }
}
