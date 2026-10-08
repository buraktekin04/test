using KLMN.Application.Common.Models.Authentication;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Application.Common.Interfaces.Authentication;

/// <summary>JWT access token ve güvenli refresh token üretim sözleşmesidir.</summary>
public interface IJwtTokenService
{
    /// <summary>
    /// generate access token işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    AccessTokenResult GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions);

    /// <summary>
    /// generate refresh token işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    RefreshTokenResult GenerateRefreshToken();

    /// <summary>
    /// hash refresh token işlemini ilgili servis sözleşmesine göre yürütür.
    /// </summary>
    string HashRefreshToken(string token);
}
