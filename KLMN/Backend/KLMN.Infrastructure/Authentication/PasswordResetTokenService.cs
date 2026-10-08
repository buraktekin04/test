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
    private const int TokenByteLength = 64;

    private readonly PasswordResetSettings _settings;
    private readonly TimeProvider _timeProvider;

    public PasswordResetTokenService(
        IOptions<PasswordResetSettings> options,
        TimeProvider timeProvider)
    {
        _settings = options.Value;
        _timeProvider = timeProvider;
    }

    public PasswordResetTokenResult GenerateToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(TokenByteLength);
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

    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}
