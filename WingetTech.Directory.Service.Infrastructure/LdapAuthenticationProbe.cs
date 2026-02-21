using System.DirectoryServices.Protocols;
using System.Net;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of authentication probing.
/// Uses the persisted <see cref="WingetTech.Directory.Service.Core.Entities.DirectorySettings"/>
/// loaded via <see cref="IDirectorySettingsService"/>; no credentials are accepted from callers.
/// </summary>
public class LdapAuthenticationProbe : IAuthenticationProbe
{
    private readonly IDirectorySettingsService _settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LdapAuthenticationProbe"/> class.
    /// </summary>
    /// <param name="settingsService">The directory settings service.</param>
    public LdapAuthenticationProbe(IDirectorySettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <inheritdoc />
    public async Task<(bool Success, string? ErrorMessage)> TestBindAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return (false, "Directory settings have not been configured.");

        try
        {
            var identifier = new LdapDirectoryIdentifier(settings.Host, settings.Port, false, false);
            var credential = new NetworkCredential(
                $"{settings.Domain}\\{settings.BindUsername}",
                settings.BindPassword);

            using var connection = new LdapConnection(identifier, credential, AuthType.Basic)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            connection.SessionOptions.ProtocolVersion = 3;
            connection.SessionOptions.SecureSocketLayer = settings.UseSsl;

            connection.Bind();

            // Validate that the configured BaseDn is reachable
            var request = new SearchRequest(
                settings.BaseDn,
                "(objectClass=*)",
                SearchScope.Base,
                "distinguishedName");

            var response = (SearchResponse)connection.SendRequest(request);

            return response.ResultCode == ResultCode.Success
                ? (true, null)
                : (false, $"BaseDn search returned: {response.ResultCode}");
        }
        catch (LdapException ex)
        {
            return (false, $"LDAP error {ex.ErrorCode}: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
