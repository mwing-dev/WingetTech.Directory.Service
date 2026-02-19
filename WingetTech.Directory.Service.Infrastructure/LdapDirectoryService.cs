using Microsoft.Extensions.Options;
using WingetTech.Directory.Service.Core.Configuration;
using WingetTech.Directory.Service.Core.Entities;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of the directory service.
/// </summary>
public class LdapDirectoryService : IDirectoryService
{
    private readonly DirectoryOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LdapDirectoryService"/> class.
    /// </summary>
    /// <param name="options">The directory configuration options.</param>
    public LdapDirectoryService(IOptions<DirectoryOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public Task<DirectoryUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<DirectoryUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<DirectoryUser>> SearchUsersAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<DirectoryGroup?> GetGroupAsync(string groupIdentifier, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<DirectoryGroup>> SearchGroupsAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<DirectoryGroup>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<DirectoryOrganizationalUnit?> GetOrganizationalUnitAsync(string distinguishedName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
