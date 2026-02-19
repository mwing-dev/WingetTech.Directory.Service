namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Request object for authentication operations.
/// </summary>
/// <param name="Username">The username for authentication.</param>
/// <param name="Password">The password for authentication.</param>
public record AuthRequestDto(
    string Username,
    string Password
);
