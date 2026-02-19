namespace WingetTech.Directory.Service.Core;

/// <summary>
/// Configuration options for connecting to a directory service.
/// </summary>
public class DirectoryOptions
{
    /// <summary>
    /// The hostname or IP address of the directory server.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// The port number for the directory service (default: 389 for LDAP, 636 for LDAPS).
    /// </summary>
    public int Port { get; set; } = 389;

    /// <summary>
    /// Indicates whether to use SSL/TLS for the connection.
    /// </summary>
    public bool UseSsl { get; set; }

    /// <summary>
    /// The base distinguished name (DN) for directory queries.
    /// </summary>
    public string BaseDn { get; set; } = string.Empty;

    /// <summary>
    /// Optional bind DN for authenticated directory operations.
    /// </summary>
    public string? BindDn { get; set; }

    /// <summary>
    /// Optional bind password for authenticated directory operations.
    /// </summary>
    public string? BindPassword { get; set; }
}
