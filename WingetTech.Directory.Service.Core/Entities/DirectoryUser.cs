namespace WingetTech.Directory.Service.Core.Entities;

/// <summary>
/// Represents a user in the directory service.
/// </summary>
public class DirectoryUser
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username or login name.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the distinguished name of the user in the directory.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user account is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets additional attributes associated with the user.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Attributes { get; set; }
}
