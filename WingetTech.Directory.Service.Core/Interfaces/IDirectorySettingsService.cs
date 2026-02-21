using WingetTech.Directory.Service.Contracts;

namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Defines operations for persisting and retrieving directory connection settings.
/// </summary>
public interface IDirectorySettingsService
{
    /// <summary>Saves (upserts) the directory settings.</summary>
    Task SaveAsync(DirectorySettingsDto dto);

    /// <summary>Returns the current directory settings, or <c>null</c> if none exist.</summary>
    Task<DirectorySettingsDto?> GetAsync();
}
