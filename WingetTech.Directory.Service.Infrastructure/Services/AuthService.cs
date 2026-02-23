using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthenticationProbe authProbe,
        AppDbContext db,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthService> logger)
    {
        _authProbe = authProbe;
        _db = db;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var isValid = await _authProbe.ValidateCredentialsAsync(
            request.Username, request.Password, cancellationToken);

        if (!isValid)
        {
            _logger.LogWarning("Failed login attempt for username {Username}", request.Username);
            return new LoginResponseDto(false, null, null, "Invalid username or password.");
        }

        var accessToken = GenerateAccessToken(request.Username);
        var (rawToken, _) = await CreateRefreshTokenAsync(request.Username, cancellationToken);

        _logger.LogInformation("Successful login for username {Username}", request.Username);
        return new LoginResponseDto(true, accessToken, rawToken, "Login successful.");
    }

    public async Task<RefreshTokenResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == tokenHash && !r.IsRevoked, cancellationToken);

        if (stored is null || stored.ExpiresUtc <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh attempt with invalid or expired token");
            return new RefreshTokenResponseDto(false, null, null, "Invalid or expired refresh token.");
        }

        // Rotate: revoke the consumed token and issue a new one
        stored.IsRevoked = true;
        var accessToken = GenerateAccessToken(stored.Username);
        var (newRawToken, _) = await CreateRefreshTokenAsync(stored.Username, cancellationToken);

        _logger.LogInformation("Token refreshed for username {Username}", stored.Username);
        return new RefreshTokenResponseDto(true, accessToken, newRawToken, "Token refreshed.");
    }

    public async Task LogoutAsync(LogoutRequestDto request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == tokenHash, cancellationToken);

        if (stored is not null)
        {
            stored.IsRevoked = true;
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Refresh token revoked for username {Username}", stored.Username);
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

    private async Task<(string RawToken, RefreshToken Entity)> CreateRefreshTokenAsync(string username, CancellationToken cancellationToken)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenHash = HashToken(rawToken);

        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenHash,
            Username = username,
            CreatedUtc = DateTime.UtcNow,
            ExpiresUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        _db.RefreshTokens.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return (rawToken, entity);
    }

    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}
