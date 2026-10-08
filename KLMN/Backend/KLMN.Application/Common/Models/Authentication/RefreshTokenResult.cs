namespace KLMN.Application.Common.Models.Authentication;

/// <summary>Yeni oluşturulmuş refresh token sonucunu temsil eder.</summary>
public sealed record RefreshTokenResult
{
    /// <summary>
    /// İstemciye yalnızca gerektiğinde verilen açık güvenlik tokenı değeridir.
    /// </summary>
    public required string Token { get; init; }
    /// <summary>
    /// Tokenın veritabanında tutulan SHA-256 hash karşılığıdır.
    /// </summary>
    public required string TokenHash { get; init; }
    /// <summary>
    /// Tokenın geçerliliğinin sona ereceği UTC zamanıdır.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
