using WingetTech.Directory.Service.Contracts;

namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Defines operations for managing database connection settings.
/// </summary>
public interface IDatabaseSettingsService
{
    /// <summary>
    /// Saves (creates or updates) the database connection settings.
    /// </summary>
    /// <param name="dto">The database settings to save.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveAsync(DatabaseSettingsDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current database connection settings.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the settings if they exist; otherwise, null.</returns>
    Task<DatabaseSettingsDto?> GetAsync(CancellationToken cancellationToken = default);
}
