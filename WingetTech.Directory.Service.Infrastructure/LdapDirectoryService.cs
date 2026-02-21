using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;
using WingetTech.Directory.Service.Core.Entities;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure;

/// <summary>
/// LDAP implementation of the directory service.
/// All connection parameters are loaded from the persisted <see cref="DirectorySettings"/>.
/// </summary>
public class LdapDirectoryService : IDirectoryService
{
    private readonly IDirectorySettingsService _settingsService;

    private static readonly string[] UserAttributes =
    [
        "objectGUID", "sAMAccountName", "displayName", "mail",
        "distinguishedName", "userAccountControl", "whenCreated", "whenChanged",
        "memberOf"
    ];

    private static readonly string[] GroupAttributes =
    [
        "objectGUID", "cn", "distinguishedName", "member"
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
        var settings = await GetRequiredSettingsAsync();
        using var connection = CreateConnection(settings);

        // objectGUID is stored as bytes; search by the hex-encoded guid string
        var filter = $"(&(objectClass=user)(objectGUID={GuidToLdapHex(userId)}))";
        var results = Search(connection, settings.BaseDn, filter, UserAttributes);

        return results.FirstOrDefault() is { } entry ? MapUser(entry) : null;
    }

    /// <inheritdoc />
    public async Task<DirectoryUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var settings = await GetRequiredSettingsAsync();
        using var connection = CreateConnection(settings);

        var filter = $"(&(objectClass=user)(sAMAccountName={EscapeLdap(username)}))";
        var results = Search(connection, settings.BaseDn, filter, UserAttributes);

        return results.FirstOrDefault() is { } entry ? MapUser(entry) : null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DirectoryUser>> SearchUsersAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        var settings = await GetRequiredSettingsAsync();
        using var connection = CreateConnection(settings);

        var escaped = EscapeLdap(searchFilter);
        var filter = $"(&(objectClass=user)(|(sAMAccountName=*{escaped}*)(displayName=*{escaped}*)))";
        var results = Search(connection, settings.BaseDn, filter, UserAttributes);

        return results.Select(MapUser).OfType<DirectoryUser>().ToList();
    }

    /// <inheritdoc />
    public async Task<DirectoryGroup?> GetGroupAsync(string groupIdentifier, CancellationToken cancellationToken = default)
    {
        var settings = await GetRequiredSettingsAsync();
        using var connection = CreateConnection(settings);

        var filter = $"(&(objectClass=group)(cn={EscapeLdap(groupIdentifier)}))";
        var results = Search(connection, settings.BaseDn, filter, GroupAttributes);

        return results.FirstOrDefault() is { } entry ? MapGroup(entry) : null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DirectoryGroup>> SearchGroupsAsync(string searchFilter, CancellationToken cancellationToken = default)
    {
        var settings = await GetRequiredSettingsAsync();
        using var connection = CreateConnection(settings);

        var escaped = EscapeLdap(searchFilter);
        var filter = $"(&(objectClass=group)(cn=*{escaped}*))";
        var results = Search(connection, settings.BaseDn, filter, GroupAttributes);

        return results.Select(MapGroup).OfType<DirectoryGroup>().ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DirectoryGroup>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var settings = await GetRequiredSettingsAsync();
        using var connection = CreateConnection(settings);

        // First fetch the user's memberOf attribute
        var userFilter = $"(&(objectClass=user)(objectGUID={GuidToLdapHex(userId)}))";
        var userEntries = Search(connection, settings.BaseDn, userFilter, ["memberOf"]);

        if (userEntries.FirstOrDefault() is not { } userEntry)
            return Array.Empty<DirectoryGroup>();

        if (userEntry.Attributes["memberOf"] is not { } memberOfAttr)
            return Array.Empty<DirectoryGroup>();

        var groups = new List<DirectoryGroup>();
        foreach (string dn in memberOfAttr.GetValues(typeof(string)))
        {
            // Retrieve each group by DN
            var groupFilter = $"(&(objectClass=group)(distinguishedName={EscapeLdap(dn)}))";
            var groupEntries = Search(connection, settings.BaseDn, groupFilter, GroupAttributes);
            groups.AddRange(groupEntries.Select(MapGroup).OfType<DirectoryGroup>());
        }

        return groups;
    }

    /// <inheritdoc />
    public async Task<DirectoryOrganizationalUnit?> GetOrganizationalUnitAsync(string distinguishedName, CancellationToken cancellationToken = default)
    {
        var settings = await GetRequiredSettingsAsync();
        using var connection = CreateConnection(settings);

        var filter = $"(&(objectClass=organizationalUnit)(distinguishedName={EscapeLdap(distinguishedName)}))";
        var results = Search(connection, settings.BaseDn, filter,
            ["distinguishedName", "ou", "description"]);

        if (results.FirstOrDefault() is not { } entry)
            return null;

        return new DirectoryOrganizationalUnit
        {
            DistinguishedName = GetStringAttribute(entry, "distinguishedName") ?? distinguishedName,
            Name = GetStringAttribute(entry, "ou") ?? string.Empty
        };
    }

    /// <inheritdoc />
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = await _settingsService.GetAsync();
            if (settings is null)
                return false;

            using var connection = CreateConnection(settings);
            // A successful RootDSE search confirms connectivity
            var request = new SearchRequest(string.Empty, "(objectClass=*)", SearchScope.Base, "namingContexts");
            var response = (SearchResponse)connection.SendRequest(request);
            return response.ResultCode == ResultCode.Success;
        }
        catch
        {
            return false;
        }
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task<WingetTech.Directory.Service.Contracts.DirectorySettingsDto> GetRequiredSettingsAsync()
    {
        var settings = await _settingsService.GetAsync();
        if (settings is null)
            throw new InvalidOperationException("Directory settings have not been configured.");
        return settings;
    }

    private static LdapConnection CreateConnection(WingetTech.Directory.Service.Contracts.DirectorySettingsDto settings)
    {
        var identifier = new LdapDirectoryIdentifier(settings.Host, settings.Port, false, false);
        var credential = new NetworkCredential(
            $"{settings.Domain}\\{settings.BindUsername}",
            settings.BindPassword);

        var connection = new LdapConnection(identifier, credential, AuthType.Basic)
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        connection.SessionOptions.ProtocolVersion = 3;
        connection.SessionOptions.SecureSocketLayer = settings.UseSsl;

        connection.Bind();
        return connection;
    }

    private static IEnumerable<SearchResultEntry> Search(
        LdapConnection connection,
        string baseDn,
        string filter,
        string[] attributes)
    {
        var request = new SearchRequest(baseDn, filter, SearchScope.Subtree, attributes);
        var response = (SearchResponse)connection.SendRequest(request);
        return response.Entries.Cast<SearchResultEntry>();
    }

    private static DirectoryUser? MapUser(SearchResultEntry entry)
    {
        var guidBytes = entry.Attributes["objectGUID"]?[0] as byte[];
        if (guidBytes is null) return null;
        var id = new Guid(guidBytes).ToString();

        // userAccountControl bit 2 (0x2) = ACCOUNTDISABLE
        var uacStr = GetStringAttribute(entry, "userAccountControl");
        var enabled = uacStr is not null
            && int.TryParse(uacStr, out var uac)
            && (uac & 0x2) == 0;

        return new DirectoryUser
        {
            Id = id,
            Username = GetStringAttribute(entry, "sAMAccountName") ?? string.Empty,
            DisplayName = GetStringAttribute(entry, "displayName"),
            Email = GetStringAttribute(entry, "mail"),
            DistinguishedName = GetStringAttribute(entry, "distinguishedName"),
            Enabled = enabled,
            CreatedAt = ParseGeneralizedTime(GetStringAttribute(entry, "whenCreated")),
            ModifiedAt = ParseGeneralizedTime(GetStringAttribute(entry, "whenChanged"))
        };
    }

    private static DirectoryGroup? MapGroup(SearchResultEntry entry)
    {
        var guidBytes = entry.Attributes["objectGUID"]?[0] as byte[];
        if (guidBytes is null) return null;
        var id = new Guid(guidBytes).ToString();

        var members = new List<string>();
        if (entry.Attributes["member"] is { } memberAttr)
        {
            foreach (string dn in memberAttr.GetValues(typeof(string)))
                members.Add(dn);
        }

        return new DirectoryGroup
        {
            Id = id,
            Name = GetStringAttribute(entry, "cn") ?? string.Empty,
            DistinguishedName = GetStringAttribute(entry, "distinguishedName"),
            Members = members
        };
    }

    private static string? GetStringAttribute(SearchResultEntry entry, string name)
    {
        if (entry.Attributes[name] is not { } attr || attr.Count == 0)
            return null;
        return attr[0] as string ?? Encoding.UTF8.GetString(attr[0] as byte[] ?? []);
    }

    /// <summary>Converts a GUID string to the hex-escaped format used in LDAP objectGUID filters.</summary>
    private static string GuidToLdapHex(string guidString)
    {
        if (!Guid.TryParse(guidString, out var guid))
            return string.Empty;
        var bytes = guid.ToByteArray();
        var sb = new StringBuilder();
        foreach (var b in bytes)
            sb.Append('\\').Append(b.ToString("X2"));
        return sb.ToString();
    }

    /// <summary>Escapes special characters in LDAP filter values (RFC 4515).</summary>
    private static string EscapeLdap(string value)
    {
        return value
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("\0", "\\00");
    }

    private static DateTime? ParseGeneralizedTime(string? value)
    {
        if (value is null) return null;
        if (DateTime.TryParseExact(value, "yyyyMMddHHmmss.fZ",
                null, System.Globalization.DateTimeStyles.AssumeUniversal, out var dt))
            return dt.ToUniversalTime();
        if (DateTime.TryParseExact(value, "yyyyMMddHHmmssZ",
                null, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            return dt.ToUniversalTime();
        return null;
    }
}
