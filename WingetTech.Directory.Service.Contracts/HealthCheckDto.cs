namespace WingetTech.Directory.Service.Contracts;

/// <summary>
/// Data transfer object representing the health status of the directory service.
/// </summary>
/// <param name="IsHealthy">Indicates whether the directory service is healthy.</param>
/// <param name="Message">Optional message describing the health status.</param>
/// <param name="Timestamp">The timestamp when the health check was performed.</param>
public record HealthCheckDto(
    bool IsHealthy,
    string? Message,
    DateTime Timestamp
);
