using System.DirectoryServices.Protocols;
using System.Net;
using WingetTech.Directory.Service.Contracts;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of authentication probing using persisted <see cref="DirectorySettings"/>.
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
    public async Task<bool> TestBindAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return false;

        try
        {
            using var connection = BuildConnection(settings);
            var bindDn = BuildBindDn(username, settings.Domain);
            connection.Bind(new NetworkCredential(bindDn, password));
            return true;
        }
        catch (LdapException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<TestBindResponseDto> TestServiceBindAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTime.UtcNow;

        var settings = await _settingsService.GetAsync();
        if (settings is null)
        {
            return new TestBindResponseDto(
                false,
                "No directory settings have been configured.",
                timestamp);
        }

        try
        {
            using var connection = BuildConnection(settings);
            var bindDn = BuildBindDn(settings.BindUsername, settings.Domain);
            connection.Bind(new NetworkCredential(bindDn, settings.BindPassword));

            // Validate BaseDn exists via a lightweight scope-base search
            var searchRequest = new SearchRequest(
                settings.BaseDn,
                "(objectClass=*)",
                SearchScope.Base,
                "distinguishedName");

            connection.SendRequest(searchRequest);

            return new TestBindResponseDto(true, null, timestamp);
        }
        catch (LdapException ex)
        {
            return new TestBindResponseDto(false, $"LDAP error ({ex.ErrorCode}): {ex.Message}", timestamp);
        }
        catch (Exception ex)
        {
            return new TestBindResponseDto(false, ex.Message, timestamp);
        }
    }

    private static LdapConnection BuildConnection(DirectorySettingsDto settings)
    {
        var identifier = new LdapDirectoryIdentifier(settings.Host, settings.Port, false, false);
        var connection = new LdapConnection(identifier)
        {
            AuthType = AuthType.Basic
        };
        connection.SessionOptions.ProtocolVersion = 3;
        connection.SessionOptions.SecureSocketLayer = settings.UseSsl;
        return connection;
    }

    private static string BuildBindDn(string username, string domain)
    {
        if (username.Contains('@') || username.Contains('\\'))
            return username;

        return string.IsNullOrWhiteSpace(domain)
            ? username
            : $"{username}@{domain}";
    }
}
