using Microsoft.EntityFrameworkCore;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Entities;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;
public class DirectorySettingsService : IDirectorySettingsService
{
    private readonly AppDbContext _db;
    private readonly IEncryptionService _encryption;

    public DirectorySettingsService(AppDbContext db, IEncryptionService encryption)
    {
        _db = db;
        _encryption = encryption;
    }
    public async Task SaveAsync(DirectorySettingsDto dto)
    {
        var existing = await _db.DirectorySettings.FirstOrDefaultAsync();

        if (existing is null)
        {
            var settings = new DirectorySettings
            {
                Id = Guid.NewGuid(),
                Host = dto.Host,
                Port = dto.Port,
                UseSsl = dto.UseSsl,
                Domain = dto.Domain,
                BaseDn = dto.BaseDn,
                BindUsername = dto.BindUsername,
                BindPassword = _encryption.Encrypt(dto.BindPassword),
                UpdatedUtc = DateTime.UtcNow
            };
            _db.DirectorySettings.Add(settings);
        }
        else
        {
            existing.Host = dto.Host;
            existing.Port = dto.Port;
            existing.UseSsl = dto.UseSsl;
            existing.Domain = dto.Domain;
            existing.BaseDn = dto.BaseDn;
            existing.BindUsername = dto.BindUsername;
            existing.BindPassword = _encryption.Encrypt(dto.BindPassword);
            existing.UpdatedUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }
    public async Task<DirectorySettingsDto?> GetAsync()
    {
        var settings = await _db.DirectorySettings.FirstOrDefaultAsync();

        if (settings is null)
            return null;

        return new DirectorySettingsDto
        {
            Host = settings.Host,
            Port = settings.Port,
            UseSsl = settings.UseSsl,
            Domain = settings.Domain,
            BaseDn = settings.BaseDn,
            BindUsername = settings.BindUsername,
            BindPassword = _encryption.Decrypt(settings.BindPassword)
        };
    }
}
