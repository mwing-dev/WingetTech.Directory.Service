namespace WingetTech.Directory.Service.Core;

/// <summary>
/// Defines operations for directory service interactions.
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Authenticates a user against the directory service.
    /// </summary>
    Task<bool> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user information from the directory service.
    /// </summary>
    Task<object?> GetUserAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves group information from the directory service.
    /// </summary>
    Task<object?> GetGroupAsync(string groupName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for users matching the specified criteria.
    /// </summary>
    Task<IEnumerable<object>> SearchUsersAsync(string searchFilter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for groups matching the specified criteria.
    /// </summary>
    Task<IEnumerable<object>> SearchGroupsAsync(string searchFilter, CancellationToken cancellationToken = default);
}
