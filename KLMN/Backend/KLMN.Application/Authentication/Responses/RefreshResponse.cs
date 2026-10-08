namespace KLMN.Application.Authentication.Responses;

/// <summary>Refresh işlemi sonucunda Angular istemcisine döndürülen session bilgisidir.</summary>
public sealed record RefreshResponse
{
    /// <summary>
    /// Korumalı API isteklerinde kullanılacak kısa ömürlü JWT'dir.
    /// </summary>
    public required string AccessToken { get; init; }
    /// <summary>
    /// JWT'nin UTC geçerlilik sonu bilgisidir.
    /// </summary>
    public required DateTime AccessTokenExpiresAt { get; init; }
    /// <summary>
    /// Yanıtın ait olduğu kimliği doğrulanmış kullanıcıdır.
    /// </summary>
    public required LoginUserResponse User { get; init; }
}
