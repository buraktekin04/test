namespace KLMN.Application.Common.Models.Authentication;

/// <summary>Başarıyla oluşturulmuş JWT access token bilgisini temsil eder.</summary>
public sealed record AccessTokenResult
{
    public required string Token { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
