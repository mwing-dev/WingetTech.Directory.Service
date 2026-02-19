namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing a directory user.
/// </summary>
/// <param name="Id">The unique identifier for the user.</param>
/// <param name="Username">The username or login name.</param>
/// <param name="DisplayName">The user's display name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="DistinguishedName">The distinguished name of the user in the directory.</param>
/// <param name="Enabled">Indicates whether the user account is enabled.</param>
/// <param name="CreatedAt">The date and time when the user was created.</param>
/// <param name="ModifiedAt">The date and time when the user was last modified.</param>
/// <param name="Attributes">Additional attributes associated with the user.</param>
public record UserDto(
    string Id,
    string Username,
    string? DisplayName,
    string? Email,
    string? DistinguishedName,
    bool Enabled,
    DateTime? CreatedAt,
    DateTime? ModifiedAt,
    IReadOnlyDictionary<string, string>? Attributes
);
