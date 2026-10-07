namespace KLMN.Application.Authentication.Models;

/// <summary>
/// Application ile API controller arasındaki authentication sonucudur.
/// Açık refresh token yalnızca HttpOnly cookie yazılabilmesi için taşınır.
/// </summary>
public sealed class AuthSessionResult
{
    public AuthSessionResponse Response { get; init; } = new();

    public string RefreshToken { get; init; } = string.Empty;

    public DateTime RefreshTokenExpiresAt { get; init; }
}
