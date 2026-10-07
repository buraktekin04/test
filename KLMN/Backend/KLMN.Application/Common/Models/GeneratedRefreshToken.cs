namespace KLMN.Application.Common.Models;

/// <summary>
/// Yeni oluşturulmuş refresh token'ın açık değeri, hash değeri
/// ve sona erme tarihini taşır.
/// </summary>
public sealed record GeneratedRefreshToken(
    string Token,
    string TokenHash,
    DateTime ExpiresAt);
