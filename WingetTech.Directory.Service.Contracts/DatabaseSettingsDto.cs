namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object for database connection settings.
/// </summary>
/// <param name="Host">The database server hostname or IP address.</param>
/// <param name="Port">The database server port number.</param>
/// <param name="DatabaseName">The name of the database.</param>
/// <param name="Username">The username used to connect to the database.</param>
/// <param name="Password">The password used to connect to the database.</param>
public record DatabaseSettingsDto(
    string Host,
    int Port,
    string DatabaseName,
    string Username,
    string Password
);
