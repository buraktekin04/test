using System.Security.Cryptography;
using System.Text;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Models.Authentication;
using KLMN.Application.Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KLMN.Infrastructure.Authentication;

/// <summary>Kriptografik parola sıfırlama token'ı üretir ve hashler.</summary>
public sealed class PasswordResetTokenService : IPasswordResetTokenService
{
    /// <summary>
    /// token byte length özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    private const int TokenByteLength = 64;

    /// <summary>
    /// JWT veya SMTP güvenlik ve bağlantı ayarlarının doğrulanmış nesnesidir.
    /// </summary>
    private readonly PasswordResetSettings _settings;
    /// <summary>
    /// Tarihler için test edilebilir UTC zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// password reset token service işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public PasswordResetTokenService(
        IOptions<PasswordResetSettings> options,
        TimeProvider timeProvider)
    {
        _settings = options.Value;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Parola sıfırlama için kriptografik rastgele tek kullanımlık token oluşturur.
    /// </summary>
    public PasswordResetTokenResult GenerateToken()
    {
        // Kriptografik rastgele sayı üreteci tarafından sağlanan token baytlarıdır.
        var randomBytes = RandomNumberGenerator.GetBytes(TokenByteLength);
        // İstemciye bir kez iletilecek rastgele ve tahmin edilemez açık token değeridir.
        var token = Base64UrlEncoder.Encode(randomBytes);

        return new PasswordResetTokenResult
        {
            Token = token,
            TokenHash = HashToken(token),
            ExpiresAt = _timeProvider
                .GetUtcNow()
                .UtcDateTime
                .AddMinutes(_settings.TokenExpirationMinutes)
        };
    }

    /// <summary>
    /// Sıfırlama tokenını SHA-256 özetine dönüştürür.
    /// </summary>
    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        // Tek yönlü SHA-256 token hash sonucudur.
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}
