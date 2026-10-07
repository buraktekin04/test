namespace KLMN.Application.Common.Models;

/// <summary>
/// Üretilmiş JWT access token sonucunu temsil eder.
/// </summary>
public sealed record AccessTokenResult(
    string Token,
    DateTime ExpiresAt);
