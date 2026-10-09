namespace KLMN.Application.Common.Options;

/// <summary>
/// Authentication modülünün hesap kilitleme kuralları ve refresh cookie
/// ayarlarını etkin ortamın appsettings dosyasından okuyan tipli modeldir.
/// </summary>
public sealed class AuthenticationSettings
{
    /// <summary>
    /// appsettings.json / appsettings.Development.json içerisindeki
    /// authentication ayarlarının bölüm adıdır.
    /// </summary>
    public const string SectionName = "Authentication";

    /// <summary>
    /// Otomatik hesap kilitlemesinden önce izin verilen hatalı giriş sayısıdır.
    /// </summary>
    public int MaxFailedAccessAttempts { get; init; } = 5;

    /// <summary>
    /// Giriş denemeleri nedeniyle oluşan geçici kilidin dakika cinsinden süresidir.
    /// </summary>
    public int LockoutMinutes { get; init; } = 15;

    /// <summary>
    /// Refresh token'ın tarayıcıda saklandığı HttpOnly cookie adıdır.
    /// Development ve Production için ayrı isim kullanılması, localhost
    /// üzerindeki eski ortam cookie'lerinin karışmasını engeller.
    /// </summary>
    public string RefreshCookieName { get; init; } = "klmn_refresh_token";

    /// <summary>
    /// Refresh cookie'nin SameSite politikasıdır: Lax, Strict veya None.
    /// HTTP Angular ile HTTPS API arasındaki geliştirme çağrıları
    /// farklı site kabul edildiğinden Development'ta None kullanılabilir.
    /// None kullanılıyorsa Origin kontrolü zorunludur ve cookie Secure'dur.
    /// </summary>
    public string RefreshCookieSameSite { get; init; } = "Lax";
}
