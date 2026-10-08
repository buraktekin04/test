using KLMN.Application.Common.Models.Authentication;

namespace KLMN.Application.Common.Interfaces.Authentication;

/// <summary>Parola sıfırlama token üretim ve hash sözleşmesidir.</summary>
public interface IPasswordResetTokenService
{
    /// <summary>
    /// generate token işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    PasswordResetTokenResult GenerateToken();
    /// <summary>
    /// hash token işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    string HashToken(string token);
}
