using System.ComponentModel.DataAnnotations;

namespace WingetTech.Directory.Service.Core.DTOs.Auth;

public sealed record LogoutRequestDto(
    [property: Required] string RefreshToken
);
