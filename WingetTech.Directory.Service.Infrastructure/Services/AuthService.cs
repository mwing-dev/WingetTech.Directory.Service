using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WingetTech.Directory.Service.Core.Configuration;
using WingetTech.Directory.Service.Core.DTOs.Auth;
using WingetTech.Directory.Service.Core.Entities;
using WingetTech.Directory.Service.Core.Interfaces;

namespace WingetTech.Directory.Service.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IAuthenticationProbe _authProbe;
    private readonly AppDbContext _db;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IAuthenticationProbe authProbe,
        AppDbContext db,
        IOptions<JwtOptions> jwtOptions)
    {
        _authProbe = authProbe;
        _db = db;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var isValid = await _authProbe.ValidateCredentialsAsync(
            request.Username, request.Password, cancellationToken);

        if (!isValid)
            return new LoginResponseDto(false, null, null, "Invalid username or password.");

        var accessToken = GenerateAccessToken(request.Username);
        var refreshToken = await CreateRefreshTokenAsync(request.Username, cancellationToken);

        return new LoginResponseDto(true, accessToken, refreshToken.Token, "Login successful.");
    }

    public async Task<RefreshTokenResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken && !r.IsRevoked, cancellationToken);

        if (stored is null || stored.ExpiresUtc <= DateTime.UtcNow)
            return new RefreshTokenResponseDto(false, null, "Invalid or expired refresh token.");

        var accessToken = GenerateAccessToken(stored.Username);

        return new RefreshTokenResponseDto(true, accessToken, "Token refreshed.");
    }

    public async Task LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken)
    {
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken, cancellationToken);

        if (stored is not null)
        {
            stored.IsRevoked = true;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private string GenerateAccessToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            [ClaimTypes.Name] = username,
            [JwtRegisteredClaimNames.Sub] = username,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Claims = claims,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }

    private async Task<RefreshToken> CreateRefreshTokenAsync(string username, CancellationToken cancellationToken)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Username = username,
            CreatedUtc = DateTime.UtcNow,
            ExpiresUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync(cancellationToken);

        return token;
    }
}
