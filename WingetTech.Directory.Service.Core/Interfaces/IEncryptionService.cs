namespace WingetTech.Directory.Service.Core.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string value);
    string Decrypt(string value);
}
