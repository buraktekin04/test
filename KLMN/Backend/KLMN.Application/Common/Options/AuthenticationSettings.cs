namespace KLMN.Application.Common.Options;

/// <summary>Kullanıcı giriş güvenliği ve hesap kilitleme ayarlarını temsil eder.</summary>
public sealed class AuthenticationSettings
{
    /// <summary>
    /// section name değerini ilgili veri veya servis sözleşmesinde taşır.
    /// </summary>
    public const string SectionName = "Authentication";
    /// <summary>
    /// Hesabın kilitlenmesinden önce izin verilen hatalı giriş sayısıdır.
    /// </summary>
    public int MaxFailedAccessAttempts { get; init; } = 5;
    /// <summary>
    /// Hesap kilidinin dakika cinsinden devam süresidir.
    /// </summary>
    public int LockoutMinutes { get; init; } = 15;
}
