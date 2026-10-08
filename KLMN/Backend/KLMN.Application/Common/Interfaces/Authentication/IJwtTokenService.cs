using KLMN.Application.Common.Models.Authentication;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Application.Common.Interfaces.Authentication;

/// <summary>JWT access token ve güvenli refresh token üretim sözleşmesidir.</summary>
public interface IJwtTokenService
{
    AccessTokenResult GenerateAccessToken(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions);

    RefreshTokenResult GenerateRefreshToken();

    string HashRefreshToken(string token);
}
