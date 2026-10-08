namespace KLMN.Application.Authentication.Responses;

/// <summary>Refresh işlemi sonucunda Angular istemcisine döndürülen session bilgisidir.</summary>
public sealed record RefreshResponse
{
    public required string AccessToken { get; init; }
    public required DateTime AccessTokenExpiresAt { get; init; }
    public required LoginUserResponse User { get; init; }
}
