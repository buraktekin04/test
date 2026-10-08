namespace KLMN.Application.Common.Models.Authentication;

/// <summary>Başarıyla oluşturulmuş JWT access token bilgisini temsil eder.</summary>
public sealed record AccessTokenResult
{
    /// <summary>
    /// İstemciye yalnızca gerektiğinde verilen açık güvenlik tokenı değeridir.
    /// </summary>
    public required string Token { get; init; }
    /// <summary>
    /// Tokenın geçerliliğinin sona ereceği UTC zamanıdır.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
