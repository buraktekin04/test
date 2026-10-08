namespace KLMN.Infrastructure.Authentication;

/// <summary>JWT access ve refresh token ayarlarını temsil eder.</summary>
public sealed class JwtSettings
{
    /// <summary>
    /// section name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public const string SectionName = "Jwt";
    /// <summary>
    /// JWT imzasında kullanılan, güvenli yapılandırmadan alınması gereken gizli anahtardır.
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
