namespace WingetTech.Directory.Service.Core.DTOs.Auth;

public sealed record LoginRequestDto(
    string Username,
    string Password
);
