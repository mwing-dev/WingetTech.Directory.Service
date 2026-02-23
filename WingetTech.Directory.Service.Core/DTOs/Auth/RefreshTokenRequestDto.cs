using System.ComponentModel.DataAnnotations;

namespace WingetTech.Directory.Service.Core.DTOs.Auth;

public sealed record RefreshTokenRequestDto(
    [property: Required] string RefreshToken
);
