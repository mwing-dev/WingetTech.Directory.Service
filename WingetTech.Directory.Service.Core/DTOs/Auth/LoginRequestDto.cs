using System.ComponentModel.DataAnnotations;

namespace WingetTech.Directory.Service.Core.DTOs.Auth;

public sealed record LoginRequestDto(
    [property: Required] string Username,
    [property: Required] string Password
);
