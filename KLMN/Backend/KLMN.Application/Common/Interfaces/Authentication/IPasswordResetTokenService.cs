using KLMN.Application.Common.Models.Authentication;

namespace KLMN.Application.Common.Interfaces.Authentication;

/// <summary>Parola sıfırlama token üretim ve hash sözleşmesidir.</summary>
public interface IPasswordResetTokenService
{
    PasswordResetTokenResult GenerateToken();
    string HashToken(string token);
}
