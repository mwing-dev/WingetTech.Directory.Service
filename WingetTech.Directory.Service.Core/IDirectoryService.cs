using WingetTech.Directory.Service.Core.Entities;

namespace WingetTech.Directory.Service.Core.Interfaces
{
    public interface IDirectoryService
    {

        Task<DirectoryUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<DirectoryUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<DirectoryUser>> SearchUsersAsync(string searchFilter, CancellationToken cancellationToken = default);

        Task<DirectoryGroup?> GetGroupAsync(string groupIdentifier, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<DirectoryGroup>> SearchGroupsAsync(string searchFilter, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<DirectoryGroup>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default);

        Task<DirectoryOrganizationalUnit?> GetOrganizationalUnitAsync(string distinguishedName, CancellationToken cancellationToken = default);

        Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
    }

};