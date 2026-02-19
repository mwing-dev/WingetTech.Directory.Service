namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Response object for authentication operations.
/// </summary>
/// <param name="Success">Indicates whether the authentication was successful.</param>
/// <param name="Message">A message describing the authentication result.</param>
/// <param name="User">The authenticated user's information (if successful).</param>
public record AuthResponseDto(
    bool Success,
    string? Message,
    UserDto? User
);
