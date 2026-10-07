using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KLMN.Application.Common.Interfaces.Security;
using KLMN.Application.Common.Models;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KLMN.Infrastructure.Authentication;

/// <summary>
/// JWT access token ve cryptographic refresh token üretir.
/// </summary>
internal sealed class JwtTokenService(
    IOptions<JwtSettings> jwtOptions)
    : IJwtTokenService
{
    private readonly JwtSettings _settings =
        jwtOptions.Value;

    /// <inheritdoc />
    public AccessTokenResult GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        var now = DateTime.UtcNow;

        var expiresAt =
            now.AddMinutes(
                _settings.AccessTokenExpirationMinutes);

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString("N")),

                new(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new(
                    ClaimTypes.Name,
                    user.UserName),

                new(
                    ClaimTypes.Email,
                    user.Email),

                new(
                    ClaimTypes.GivenName,
                    user.FirstName),

                new(
                    ClaimTypes.Surname,
                    user.LastName),

                new(
                    CustomClaimTypes.SecurityStamp,
                    user.SecurityStamp)
            };

        if (user.OrganizationUnitId.HasValue)
        {
            claims.Add(
                new Claim(
                    CustomClaimTypes.OrganizationUnitId,
                    user.OrganizationUnitId.Value.ToString()));
        }

        claims.AddRange(
            roles
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(
                    role =>
                        new Claim(
                            ClaimTypes.Role,
                            role)));

        claims.AddRange(
            permissions
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(
                    permission =>
                        new Claim(
                            CustomClaimTypes.Permission,
                            permission)));

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _settings.SecretKey));

        var credentials =
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

        return new AccessTokenResult(
            new JwtSecurityTokenHandler()
                .WriteToken(token),
            expiresAt);
    }

    /// <inheritdoc />
    public GeneratedRefreshToken GenerateRefreshToken()
    {
        var bytes =
            RandomNumberGenerator.GetBytes(64);

        var token =
            Convert.ToBase64String(bytes);

        var expiresAt =
            DateTime.UtcNow.AddDays(
                _settings.RefreshTokenExpirationDays);

        return new GeneratedRefreshToken(
            token,
            HashRefreshToken(token),
            expiresAt);
    }

    /// <inheritdoc />
    public string HashRefreshToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}
