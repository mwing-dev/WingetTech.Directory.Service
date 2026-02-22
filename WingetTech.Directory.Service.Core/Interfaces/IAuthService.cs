using WingetTech.Directory.Service.Core.DTOs.Auth;

namespace WingetTech.Directory.Service.Core.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
    Task<RefreshTokenResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken);
    Task LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken);
}
