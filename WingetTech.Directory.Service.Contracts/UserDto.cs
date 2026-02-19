namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing a directory user.
/// </summary>
public class UserDto
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The username or login name.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The user's display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// The user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// The user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// The distinguished name of the user in the directory.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// Groups the user belongs to.
    /// </summary>
    public List<string> Groups { get; set; } = new();

    /// <summary>
    /// Indicates whether the user account is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }
}
