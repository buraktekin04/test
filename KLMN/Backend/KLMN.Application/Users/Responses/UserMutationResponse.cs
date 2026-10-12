namespace KLMN.Application.Users.Responses;

/// <summary>
/// Kullanıcı oluşturma, güncelleme ve aktiflik işlemlerinden sonra
/// istemciye döndürülen kimlik ve eşzamanlılık bilgilerini taşır.
/// </summary>
public sealed record UserMutationResponse
{
    /// <summary>Kullanıcının benzersiz kaydıdır.</summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// PostgreSQL xmin sistem kolonunun güncel değeridir.
    /// Sonraki değişiklikte concurrency kontrolü için gönderilir.
    /// </summary>
    public required uint Version { get; init; }
}
