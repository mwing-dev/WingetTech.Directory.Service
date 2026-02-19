using Microsoft.Extensions.Options;
using WingetTech.Directory.Service.Core;
using WingetTech.Directory.Service.Contracts;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of the directory service.
/// </summary>
public class LdapDirectoryService : IDirectoryService
{
    private readonly DirectoryOptions _options;

    public LdapDirectoryService(IOptions<DirectoryOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public Task<bool> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        // TODO: Implement LDAP authentication
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<object?> GetUserAsync(string username, CancellationToken cancellationToken = default)
    {
        // TODO: Implement LDAP user retrieval
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<object?> GetGroupAsync(string groupName, CancellationToken cancellationToken = default)
    {
        // TODO: Implement LDAP group retrieval
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IEnumerable<object>> SearchUsersAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        // TODO: Implement LDAP user search
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IEnumerable<object>> SearchGroupsAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        // TODO: Implement LDAP group search
        throw new NotImplementedException();
    }
}
