using System.Security.Cryptography;
using System.Text;
using KLMN.Application.Common.Interfaces.Security;
using KLMN.Application.Common.Models;
using KLMN.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace KLMN.Infrastructure.Authentication;

/// <summary>
/// Güvenli parola sıfırlama token'ları üretir ve hashler.
/// </summary>
internal sealed class PasswordResetTokenService(
    IOptions<PasswordResetSettings> passwordResetOptions)
    : IPasswordResetTokenService
{
    private readonly PasswordResetSettings _settings =
        passwordResetOptions.Value;

    /// <inheritdoc />
    public GeneratedPasswordResetToken GenerateToken()
    {
        var bytes =
            RandomNumberGenerator.GetBytes(48);

        var token =
            Convert.ToBase64String(bytes);

        return new GeneratedPasswordResetToken(
            token,
            HashToken(token),
            DateTime.UtcNow.AddMinutes(
                _settings.TokenExpirationMinutes));
    }

    /// <inheritdoc />
    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}
