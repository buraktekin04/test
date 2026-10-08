namespace KLMN.Infrastructure.Authentication;

/// <summary>JWT access ve refresh token ayarlarını appsettings.json veya appsettings.Development.json dosyasındaki Jwt bölümünden tip güvenli olarak okuyan modeldir; bağımsız bir ayar dosyası değildir.</summary>
public sealed class JwtSettings
{
    /// <summary>
    /// section name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public const string SectionName = "Jwt";
    /// <summary>
    /// etkin ortamın Jwt:SecretKey alanından okunan JWT imzalama anahtarıdır. En az 32 byte olmalıdır.
    /// </summary>
    public string SecretKey { get; init; } = string.Empty;
    /// <summary>
    /// JWT'yi üreten güvenilir sunucunun kimliğidir.
    /// </summary>
    public string Issuer { get; init; } = string.Empty;
    /// <summary>
    /// Tokenın kullanılmasının beklendiği API veya istemci kimliğidir.
    /// </summary>
    public string Audience { get; init; } = string.Empty;
    /// <summary>
    /// JWT'nin dakika cinsinden kısa geçerlilik süresidir.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    /// <summary>
    /// Refresh cookie'sinin gün cinsinden oturum yenileme süresidir.
    /// </summary>
    public int RefreshTokenExpirationDays { get; init; } = 7;
}
