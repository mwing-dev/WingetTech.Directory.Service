using System.DirectoryServices.Protocols;
using System.Net;
using WingetTech.Directory.Service.Core.Entities;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of the directory service using persisted <see cref="DirectorySettings"/>.
/// </summary>
public class LdapDirectoryService : IDirectoryService
{
    private readonly IDirectorySettingsService _settingsService;

    private static readonly string[] UserAttributes =
    [
        "objectGUID", "sAMAccountName", "displayName", "mail",
        "distinguishedName", "userAccountControl", "memberOf",
        "whenCreated", "whenChanged"
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="LdapDirectoryService"/> class.
    /// </summary>
    /// <param name="settingsService">The directory settings service.</param>
    public LdapDirectoryService(IDirectorySettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <inheritdoc />
    public async Task<DirectoryUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return null;

        // Convert GUID string to octet-string filter format for objectGUID
        if (!Guid.TryParse(userId, out var guid))
            return null;

        var octetFilter = BuildGuidFilter(guid);
        var filter = $"(&(objectClass=user)(objectGUID={octetFilter}))";

        return await SearchSingleUserAsync(settings, filter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DirectoryUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return null;

        var escapedUsername = EscapeLdapFilter(username);
        var filter = $"(&(objectClass=user)(sAMAccountName={escapedUsername}))";

        return await SearchSingleUserAsync(settings, filter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DirectoryUser>> SearchUsersAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return Array.Empty<DirectoryUser>();

        var escaped = EscapeLdapFilter(searchFilter);
        var filter = $"(&(objectClass=user)(|(sAMAccountName=*{escaped}*)(displayName=*{escaped}*)))";

        return await SearchUsersInternalAsync(settings, filter, cancellationToken);
    }

    private static readonly string[] GroupAttributes =
    [
        "objectGUID", "cn", "displayName", "distinguishedName",
        "description", "member"
    ];

    /// <inheritdoc />
    public async Task<DirectoryGroup?> GetGroupAsync(string groupIdentifier, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return null;

        string filter;
        if (Guid.TryParse(groupIdentifier, out var guid))
        {
            var octetFilter = BuildGuidFilter(guid);
            filter = $"(&(objectClass=group)(objectGUID={octetFilter}))";
        }
        else
        {
            var escapedDn = EscapeLdapFilter(groupIdentifier);
            filter = $"(&(objectClass=group)(distinguishedName={escapedDn}))";
        }

        return await SearchSingleGroupAsync(settings, filter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DirectoryGroup>> SearchGroupsAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return Array.Empty<DirectoryGroup>();

        var escaped = EscapeLdapFilter(searchFilter);
        var filter = $"(&(objectClass=group)(|(cn=*{escaped}*)(displayName=*{escaped}*)))";

        return await SearchGroupsInternalAsync(settings, filter, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DirectoryGroup>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return Array.Empty<DirectoryGroup>();

        if (!Guid.TryParse(userId, out var guid))
            return Array.Empty<DirectoryGroup>();

        var octetFilter = BuildGuidFilter(guid);
        var filter = $"(&(objectClass=user)(objectGUID={octetFilter}))";

        // First get the user to retrieve their memberOf attribute
        using var connection = BuildConnection(settings);
        Bind(connection, settings);

        var searchRequest = new SearchRequest(
            settings.BaseDn,
            filter,
            SearchScope.Subtree,
            "memberOf");

        var response = (SearchResponse)connection.SendRequest(searchRequest);

        if (response.Entries.Count == 0)
            return Array.Empty<DirectoryGroup>();

        var entry = response.Entries[0];
        var memberOfAttr = entry.Attributes["memberOf"];
        if (memberOfAttr is null)
            return Array.Empty<DirectoryGroup>();

        var groups = new List<DirectoryGroup>();
        foreach (string groupDn in memberOfAttr.GetValues(typeof(string)).Cast<string>())
        {
            var cn = ExtractCn(groupDn);
            groups.Add(new DirectoryGroup
            {
                Id = groupDn,
                Name = cn,
                DistinguishedName = groupDn,
                Members = Array.Empty<string>()
            });
        }

        return groups;
    }

    /// <inheritdoc />
    public Task<DirectoryOrganizationalUnit?> GetOrganizationalUnitAsync(string distinguishedName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            return false;

        try
        {
            using var connection = BuildConnection(settings);
            Bind(connection, settings);
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

    private async Task<DirectoryGroup?> SearchSingleGroupAsync(
        Contracts.DirectorySettingsDto settings,
        string filter,
        CancellationToken cancellationToken)
    {
        var results = await SearchGroupsInternalAsync(settings, filter, cancellationToken);
        return results.FirstOrDefault();
    }

    private Task<IReadOnlyCollection<DirectoryGroup>> SearchGroupsInternalAsync(
        Contracts.DirectorySettingsDto settings,
        string filter,
        CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            using var connection = BuildConnection(settings);
            Bind(connection, settings);

            var searchRequest = new SearchRequest(
                settings.BaseDn,
                filter,
                SearchScope.Subtree,
                GroupAttributes);

            var response = (SearchResponse)connection.SendRequest(searchRequest);

            var groups = new List<DirectoryGroup>();
            foreach (SearchResultEntry entry in response.Entries)
            {
                groups.Add(MapToDirectoryGroup(entry));
            }

            return (IReadOnlyCollection<DirectoryGroup>)groups;
        }, cancellationToken);
    }

    private static DirectoryGroup MapToDirectoryGroup(SearchResultEntry entry)
    {
        var objectGuidBytes = entry.Attributes["objectGUID"]?[0] as byte[];
        var objectGuid = objectGuidBytes is not null
            ? new Guid(objectGuidBytes).ToString()
            : string.Empty;

        var members = new List<string>();
        var memberAttr = entry.Attributes["member"];
        if (memberAttr is not null)
        {
            foreach (string dn in memberAttr.GetValues(typeof(string)).Cast<string>())
                members.Add(dn);
        }

        return new DirectoryGroup
        {
            Id = string.IsNullOrEmpty(objectGuid) ? entry.DistinguishedName : objectGuid,
            Name = GetStringAttribute(entry, "cn"),
            DistinguishedName = entry.DistinguishedName,
            Description = GetStringAttribute(entry, "description") is { Length: > 0 } desc ? desc : null,
            Members = members
        };
    }

    private async Task<DirectoryUser?> SearchSingleUserAsync(
        Contracts.DirectorySettingsDto settings,
        string filter,
        CancellationToken cancellationToken)
    {
        var results = await SearchUsersInternalAsync(settings, filter, cancellationToken);
        return results.FirstOrDefault();
    }

    private Task<IReadOnlyCollection<DirectoryUser>> SearchUsersInternalAsync(
        Contracts.DirectorySettingsDto settings,
        string filter,
        CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            using var connection = BuildConnection(settings);
            Bind(connection, settings);

            var searchRequest = new SearchRequest(
                settings.BaseDn,
                filter,
                SearchScope.Subtree,
                UserAttributes);

            var response = (SearchResponse)connection.SendRequest(searchRequest);

            var users = new List<DirectoryUser>();
            foreach (SearchResultEntry entry in response.Entries)
            {
                users.Add(MapToDirectoryUser(entry));
            }

            return (IReadOnlyCollection<DirectoryUser>)users;
        }, cancellationToken);
    }

    private static LdapConnection BuildConnection(Contracts.DirectorySettingsDto settings)
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

    private static void Bind(LdapConnection connection, Contracts.DirectorySettingsDto settings)
    {
        var credential = new NetworkCredential(
            BuildBindDn(settings),
            settings.BindPassword);
        connection.Bind(credential);
    }

    private static string BuildBindDn(Contracts.DirectorySettingsDto settings)
    {
        // Support both UPN (user@domain) and pre-configured BindUsername
        if (settings.BindUsername.Contains('@') || settings.BindUsername.Contains('\\'))
            return settings.BindUsername;

        return string.IsNullOrWhiteSpace(settings.Domain)
            ? settings.BindUsername
            : $"{settings.BindUsername}@{settings.Domain}";
    }

    private static DirectoryUser MapToDirectoryUser(SearchResultEntry entry)
    {
        var objectGuidBytes = entry.Attributes["objectGUID"]?[0] as byte[];
        var objectGuid = objectGuidBytes is not null
            ? new Guid(objectGuidBytes).ToString()
            : string.Empty;

        var uac = GetIntAttribute(entry, "userAccountControl");
        // Bit 2 (0x2) = account disabled; uac == 0 means attribute absent — treat as disabled
        var enabled = uac != 0 && (uac & 0x2) == 0;

        return new DirectoryUser
        {
            Id = objectGuid,
            Username = GetStringAttribute(entry, "sAMAccountName"),
            DisplayName = GetStringAttribute(entry, "displayName"),
            Email = GetStringAttribute(entry, "mail"),
            DistinguishedName = entry.DistinguishedName,
            Enabled = enabled,
            CreatedAt = GetDateAttribute(entry, "whenCreated"),
            ModifiedAt = GetDateAttribute(entry, "whenChanged"),
            Attributes = null
        };
    }

    private static string GetStringAttribute(SearchResultEntry entry, string name)
    {
        var attr = entry.Attributes[name];
        if (attr is null || attr.Count == 0)
            return string.Empty;
        return attr[0] as string ?? string.Empty;
    }

    private static int GetIntAttribute(SearchResultEntry entry, string name)
    {
        var raw = GetStringAttribute(entry, name);
        return int.TryParse(raw, out var val) ? val : 0;
    }

    private static DateTime? GetDateAttribute(SearchResultEntry entry, string name)
    {
        var raw = GetStringAttribute(entry, name);
        if (string.IsNullOrEmpty(raw))
            return null;

        // AD format: yyyyMMddHHmmss.0Z
        if (DateTime.TryParseExact(raw, "yyyyMMddHHmmss.0'Z'",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
            out var dt))
        {
            return dt;
        }

        return null;
    }

    /// <summary>
    /// Encodes a GUID as an LDAP octet-string filter component for objectGUID searches.
    /// </summary>
    private static string BuildGuidFilter(Guid guid)
    {
        var bytes = guid.ToByteArray();
        return string.Concat(bytes.Select(b => $"\\{b:x2}"));
    }

    /// <summary>
    /// Escapes special characters in an LDAP filter value per RFC 4515.
    /// </summary>
    private static string EscapeLdapFilter(string value)
    {
        return value
            .Replace("\\", "\\5c")
            .Replace("*",  "\\2a")
            .Replace("(",  "\\28")
            .Replace(")",  "\\29")
            .Replace("\0", "\\00");
    }

    private static string ExtractCn(string distinguishedName)
    {
        var parts = distinguishedName.Split(',');
        if (parts.Length > 0 && parts[0].StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
            return parts[0][3..];
        return distinguishedName;
    }
}
