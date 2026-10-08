namespace KLMN.Application.Common.Models.Authentication;

/// <summary>Yeni oluşturulmuş refresh token sonucunu temsil eder.</summary>
public sealed record RefreshTokenResult
{
    public required string Token { get; init; }
    public required string TokenHash { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
