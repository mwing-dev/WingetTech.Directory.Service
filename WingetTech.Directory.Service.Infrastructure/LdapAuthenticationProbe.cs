using Microsoft.Extensions.Options;
using WingetTech.Directory.Service.Core.Configuration;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of authentication probing.
/// </summary>
public class LdapAuthenticationProbe : IAuthenticationProbe
{
    private readonly DirectoryOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LdapAuthenticationProbe"/> class.
    /// </summary>
    /// <param name="options">The directory configuration options.</param>
    public LdapAuthenticationProbe(IOptions<DirectoryOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public Task<bool> TestBindAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
