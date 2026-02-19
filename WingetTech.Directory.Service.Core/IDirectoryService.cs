using WingetTech.Directory.Service.Core.Entities;

namespace WingetTech.Directory.Service.Core.Interfaces;

/// <summary>
/// Defines operations for directory service interactions.
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<DirectoryUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<DirectoryUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for users matching the specified criteria.
    /// </summary>
    /// <param name="searchFilter">The search filter to apply.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of matching users.</returns>
    Task<IReadOnlyCollection<DirectoryUser>> SearchUsersAsync(string searchFilter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a group by its name or identifier.
    /// </summary>
    /// <param name="groupIdentifier">The name or identifier of the group.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the group if found; otherwise, null.</returns>
    Task<DirectoryGroup?> GetGroupAsync(string groupIdentifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for groups matching the specified criteria.
    /// </summary>
    /// <param name="searchFilter">The search filter to apply.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of matching groups.</returns>
    Task<IReadOnlyCollection<DirectoryGroup>> SearchGroupsAsync(string searchFilter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all groups that a user is a member of.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of groups the user belongs to.</returns>
    Task<IReadOnlyCollection<DirectoryGroup>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an organizational unit by its distinguished name.
    /// </summary>
    /// <param name="distinguishedName">The distinguished name of the organizational unit.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the organizational unit if found; otherwise, null.</returns>
    Task<DirectoryOrganizationalUnit?> GetOrganizationalUnitAsync(string distinguishedName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a health check on the directory service connection.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the service is healthy.</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}
