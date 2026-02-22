using WingetTech.Directory.Service.Core.DTOs.Auth;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    public Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<RefreshTokenResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}
