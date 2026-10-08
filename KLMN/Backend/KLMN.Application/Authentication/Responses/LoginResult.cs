namespace KLMN.Application.Authentication.Responses;

/// <summary>
/// Login handler tarafından API katmanına döndürülen authentication sonucudur.
/// Refresh token response body'ye yazılmaz; controller tarafından HttpOnly cookie'ye yazılır.
/// </summary>
public sealed record LoginResult
{
    public required LoginResponse Response { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime RefreshTokenExpiresAt { get; init; }
}
