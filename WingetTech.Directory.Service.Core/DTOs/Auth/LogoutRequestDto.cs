namespace WingetTech.Directory.Service.Core.DTOs.Auth;

public sealed record LogoutRequestDto(
    string RefreshToken
);
