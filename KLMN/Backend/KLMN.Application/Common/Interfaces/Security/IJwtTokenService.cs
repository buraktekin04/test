using KLMN.Application.Common.Models;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Application.Common.Interfaces.Security;

/// <summary>
/// Access ve refresh token üretim işlemlerini tanımlar.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Kullanıcı için yeni JWT access token üretir.
    /// </summary>
    AccessTokenResult GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions);

    /// <summary>
    /// Yeni cryptographic refresh token üretir.
    /// </summary>
    GeneratedRefreshToken GenerateRefreshToken();

    /// <summary>
    /// Refresh token değerini SHA-256 ile hashler.
    /// </summary>
    string HashRefreshToken(string token);
}
