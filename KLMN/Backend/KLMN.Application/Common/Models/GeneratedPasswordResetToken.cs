namespace KLMN.Application.Common.Models;

/// <summary>
/// Yeni oluşturulmuş parola sıfırlama token sonucudur.
/// </summary>
public sealed record GeneratedPasswordResetToken(
    string Token,
    string TokenHash,
    DateTime ExpiresAt);
