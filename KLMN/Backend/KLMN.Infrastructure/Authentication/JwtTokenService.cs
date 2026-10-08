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
    /// <summary>
    /// refresh token byte length özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    private const int RefreshTokenByteLength = 64;

    /// <summary>
    /// JWT veya SMTP güvenlik ve bağlantı ayarlarının doğrulanmış nesnesidir.
    /// </summary>
    private readonly JwtSettings _settings;
    /// <summary>
    /// Tarihler için test edilebilir UTC zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;
    /// <summary>
    /// JWT tokenına HMAC-SHA256 imzası eklemek için kullanılan kimlik bilgileridir.
    /// </summary>
    private readonly SigningCredentials _signingCredentials;

    /// <summary>
    /// jwt token service işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public JwtTokenService(
        IOptions<JwtSettings> options,
        TimeProvider timeProvider)
    {
        _settings = options.Value;
        _timeProvider = timeProvider;

        // JWT imzasında kullanılmak üzere oluşturulmuş simetrik güvenlik anahtarıdır.
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey));

        _signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);
    }

    /// <summary>
    /// Kullanıcı rol ve etkin izin claimlerini imzalayarak kısa ömürlü JWT erişim tokenı üretir.
    /// </summary>
    public AccessTokenResult GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);
        ArgumentNullException.ThrowIfNull(permissions);

        // Token bitiş zamanlarını UTC üzerinden hesaplamak için güncel zamandır.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        // Oluşturulan tokenın geçerliliğinin sona ereceği UTC zamanıdır.
        var expiresAt = utcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        // İstemciye bir kez iletilecek rastgele ve tahmin edilemez açık token değeridir.
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

    /// <summary>
    /// Kriptografik rastgele token üretir; açık değer ve saklanacak hashini birlikte döndürür.
    /// </summary>
    public RefreshTokenResult GenerateRefreshToken()
    {
        // Kriptografik rastgele sayı üreteci tarafından sağlanan token baytlarıdır.
        var randomBytes = RandomNumberGenerator.GetBytes(RefreshTokenByteLength);
        // İstemciye bir kez iletilecek rastgele ve tahmin edilemez açık token değeridir.
        var token = Base64UrlEncoder.Encode(randomBytes);
        // Oluşturulan tokenın geçerliliğinin sona ereceği UTC zamanıdır.
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

    /// <summary>
    /// Açık refresh tokenı SHA-256 hashine dönüştürür.
    /// </summary>
    public string HashRefreshToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        // Açık tokenın SHA-256 işleminden dönen ikili özetidir.
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
        // JWT'ye eklenecek kullanıcı kimlik ve yetki iddialarının koleksiyonudur.
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
