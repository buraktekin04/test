namespace KLMN.Application.Authentication.Models;

/// <summary>
/// Angular istemcisine döndürülen authentication session cevabıdır.
/// Refresh token response body içerisinde bulunmaz.
/// </summary>
public sealed class AuthSessionResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime AccessTokenExpiresAt { get; init; }

    public AuthUserResponse User { get; init; } = new();
}
