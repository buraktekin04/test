using KLMN.Application.Common.Models;

namespace KLMN.Application.Common.Interfaces.Security;

/// <summary>
/// Parola sıfırlama token üretim ve hash işlemlerini tanımlar.
/// </summary>
public interface IPasswordResetTokenService
{
    GeneratedPasswordResetToken GenerateToken();

    string HashToken(string token);
}
