namespace KLMN.Application.Authentication.Responses;

/// <summary>
/// Login handler tarafından API katmanına döndürülen authentication sonucudur.
/// Refresh token response body'ye yazılmaz; controller tarafından HttpOnly cookie'ye yazılır.
/// </summary>
public sealed record LoginResult
{
    /// <summary>
    /// Refresh tokenı açığa çıkarmadan Angular'a dönen yanıt gövdesidir.
    /// </summary>
    public required LoginResponse Response { get; init; }
    /// <summary>
    /// Yalnızca HttpOnly cookie ile taşınan açık refresh token değeridir.
    /// </summary>
    public required string RefreshToken { get; init; }
    /// <summary>
    /// HttpOnly yenileme cookie'sinin son geçerlilik zamanıdır.
    /// </summary>
    public required DateTime RefreshTokenExpiresAt { get; init; }
}
