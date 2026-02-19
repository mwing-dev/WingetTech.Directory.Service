namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Response object for authentication operations.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Indicates whether the authentication was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A message describing the authentication result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The authenticated user's information (if successful).
    /// </summary>
    public UserDto? User { get; set; }
}
