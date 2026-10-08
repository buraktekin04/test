namespace KLMN.Application.Authentication.Responses;

/// <summary>Refresh handler tarafından API katmanına gönderilen sonucu temsil eder.</summary>
public sealed record RefreshResult
{
    /// <summary>
    /// Refresh tokenı açığa çıkarmadan Angular'a dönen yanıt gövdesidir.
    /// </summary>
    public required RefreshResponse Response { get; init; }
    /// <summary>
    /// Yalnızca HttpOnly cookie ile taşınan açık refresh token değeridir.
    /// </summary>
    public required string RefreshToken { get; init; }
    /// <summary>
    /// HttpOnly yenileme cookie'sinin son geçerlilik zamanıdır.
    /// </summary>
    public required DateTime RefreshTokenExpiresAt { get; init; }
}
