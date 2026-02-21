using System.DirectoryServices.Protocols;
using System.Net;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of authentication probing.
/// </summary>
public class LdapAuthenticationProbe : IAuthenticationProbe
{
    private readonly IDirectorySettingsService _directorySettingsService;

    public LdapAuthenticationProbe(IDirectorySettingsService directorySettingsService)
    {
        _directorySettingsService = directorySettingsService;
    }

    /// <inheritdoc />
    public async Task<bool> TestBindAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _directorySettingsService.GetAsync();

        if (settings is null)
            throw new InvalidOperationException("Directory settings are not configured.");

        var identifier = new LdapDirectoryIdentifier(settings.Host, settings.Port);

        using var connection = new LdapConnection(identifier)
        {
            AuthType = AuthType.Basic
        };

        connection.SessionOptions.ProtocolVersion = 3;

        if (settings.UseSsl)
            connection.SessionOptions.SecureSocketLayer = true;

        var credential = new NetworkCredential(
            settings.BindUsername,
            settings.BindPassword);

        try
        {
            connection.Bind(credential);

            // Optional: verify BaseDn exists
            var request = new SearchRequest(
                settings.BaseDn,
                "(objectClass=*)",
                SearchScope.Base,
                null);

            connection.SendRequest(request);

            return true;
        }
        catch (LdapException)
        {
            return false;
        }
    }
}