namespace WingetTech.Directory.Service.Contracts
{
    public record TestBindResponseDto(
        bool Success,
        string? ErrorMessage,
        DateTime Timestamp
    );
}
