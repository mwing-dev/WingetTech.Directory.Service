namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Response object for the test-bind diagnostics endpoint.
/// </summary>
/// <param name="Success">Indicates whether the LDAP bind was successful.</param>
/// <param name="ErrorMessage">An error message if the bind failed; otherwise, null.</param>
/// <param name="Timestamp">The UTC timestamp when the test was performed.</param>
public record TestBindResponseDto(
    bool Success,
    string? ErrorMessage,
    DateTime Timestamp
);
