namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Request object for authentication operations.
/// </summary>
public class AuthRequestDto
{
    /// <summary>
    /// The username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
