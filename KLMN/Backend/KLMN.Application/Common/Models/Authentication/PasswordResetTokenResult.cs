namespace KLMN.Application.Common.Models.Authentication;

/// <summary>Yeni oluşturulmuş parola sıfırlama token sonucunu temsil eder.</summary>
public sealed record PasswordResetTokenResult
{
    public required string Token { get; init; }
    public required string TokenHash { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
