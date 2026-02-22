namespace WingetTech.Directory.Service.Contracts
{
    public record HealthCheckDto(
        bool IsHealthy,
        string? Message,
        DateTime Timestamp
    );
}
