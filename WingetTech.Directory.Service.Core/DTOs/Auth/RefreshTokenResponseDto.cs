namespace WingetTech.Directory.Service.Core.DTOs.Auth;

public sealed record RefreshTokenResponseDto(
    bool Success,
    string? AccessToken,
    string? RefreshToken,
    string Message
);
