using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KLMN.Application.Common.Constants;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Models.Authentication;
using KLMN.Domain.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KLMN.Infrastructure.Authentication;

/// <summary>
/// HMAC SHA-256 ile JWT access token ve güvenli refresh token üretir.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private const int RefreshTokenByteLength = 64;

    private readonly JwtSettings _settings;
    private readonly TimeProvider _timeProvider;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(
        IOptions<JwtSettings> options,
        TimeProvider timeProvider)
    {
        _settings = options.Value;
        _timeProvider = timeProvider;

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey));

        _signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);
    }

    public AccessTokenResult GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);
        ArgumentNullException.ThrowIfNull(permissions);

        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = utcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: CreateUserClaims(user, roles, permissions),
            notBefore: utcNow,
            expires: expiresAt,
            signingCredentials: _signingCredentials);

        return new AccessTokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    public RefreshTokenResult GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(RefreshTokenByteLength);
        var token = Base64UrlEncoder.Encode(randomBytes);
        var expiresAt = _timeProvider
            .GetUtcNow()
            .UtcDateTime
            .AddDays(_settings.RefreshTokenExpirationDays);

        return new RefreshTokenResult
        {
            Token = token,
            TokenHash = HashRefreshToken(token),
            ExpiresAt = expiresAt
        };
    }

    public string HashRefreshToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hashBytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// JWT claim koleksiyonunu oluşturur.
    /// SecurityStamp claim'i özellikle eklenir; OnTokenValidated bu değeri
    /// DB'deki güncel SecurityStamp ile karşılaştırır.
    /// </summary>
    private static List<Claim> CreateUserClaims(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(CustomClaimTypes.SecurityStamp, user.SecurityStamp)
        };

        if (user.OrganizationUnitId.HasValue)
        {
            claims.Add(
                new Claim(
                    CustomClaimTypes.OrganizationUnitId,
                    user.OrganizationUnitId.Value.ToString()));
        }

        foreach (var role in roles
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(CustomClaimTypes.Permission, permission));
        }

        return claims;
    }
}
