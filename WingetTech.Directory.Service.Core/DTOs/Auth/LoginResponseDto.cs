namespace WingetTech.Directory.Service.Core.DTOs.Auth;

public sealed record LoginResponseDto(
    bool Success,
    string? AccessToken,
    string? RefreshToken,
    string Message
);
