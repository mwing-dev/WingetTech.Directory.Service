using Microsoft.EntityFrameworkCore;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Entities;
using WingetTech.Directory.Service.Core.Interfaces;
using WingetTech.Directory.Service.Infrastructure.Persistence;

namespace WingetTech.Directory.Service.Infrastructure.Services;

/// <summary>
/// Service for managing database connection settings.
/// Treats the settings table as a singleton row: if a row exists it is updated, otherwise a new row is created.
/// </summary>
public class DatabaseSettingsService : IDatabaseSettingsService
{
    private readonly DirectoryDbContext _context;
    private readonly IEncryptionService _encryptionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseSettingsService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="encryptionService">The encryption service.</param>
    public DatabaseSettingsService(DirectoryDbContext context, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    /// <inheritdoc />
    public async Task SaveAsync(DatabaseSettingsDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _context.DatabaseSettings.FirstOrDefaultAsync(cancellationToken);

        if (existing is null)
        {
            var settings = new DatabaseSettings
            {
                Id = Guid.NewGuid(),
                Host = _encryptionService.Encrypt(dto.Host),
                Port = dto.Port,
                DatabaseName = _encryptionService.Encrypt(dto.DatabaseName),
                Username = _encryptionService.Encrypt(dto.Username),
                Password = _encryptionService.Encrypt(dto.Password),
                UpdatedUtc = DateTime.UtcNow
            };
            _context.DatabaseSettings.Add(settings);
        }
        else
        {
            existing.Host = _encryptionService.Encrypt(dto.Host);
            existing.Port = dto.Port;
            existing.DatabaseName = _encryptionService.Encrypt(dto.DatabaseName);
            existing.Username = _encryptionService.Encrypt(dto.Username);
            existing.Password = _encryptionService.Encrypt(dto.Password);
            existing.UpdatedUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DatabaseSettingsDto?> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _context.DatabaseSettings.FirstOrDefaultAsync(cancellationToken);

        if (settings is null)
            return null;

        return new DatabaseSettingsDto(
            _encryptionService.Decrypt(settings.Host),
            settings.Port,
            _encryptionService.Decrypt(settings.DatabaseName),
            _encryptionService.Decrypt(settings.Username),
            _encryptionService.Decrypt(settings.Password)
        );
    }
}
