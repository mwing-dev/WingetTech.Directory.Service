namespace WingetTech.Directory.Service.Core.Entities;

/// <summary>
/// Represents the database connection settings (singleton row).
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the database server hostname or IP address.
    /// </summary>
    public string Host { get; set; } = default!;

    /// <summary>
    /// Gets or sets the database server port number.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    public string DatabaseName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the username used to connect to the database.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Gets or sets the password used to connect to the database.
    /// </summary>
    public string Password { get; set; } = default!;

    /// <summary>
    /// Gets or sets the UTC timestamp of the last update.
    /// </summary>
    public DateTime UpdatedUtc { get; set; }
}
