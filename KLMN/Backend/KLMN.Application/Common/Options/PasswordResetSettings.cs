namespace KLMN.Application.Common.Options;

/// <summary>Parola sıfırlama token süre ve istemci yönlendirme ayarlarını temsil eder.</summary>
public sealed class PasswordResetSettings
{
    /// <summary>
    /// section name değerini ilgili veri veya servis sözleşmesinde taşır.
    /// </summary>
    public const string SectionName = "PasswordReset";
    /// <summary>
    /// Sıfırlama bağlantısının dakika cinsinden geçerlilik süresidir.
    /// </summary>
    public int TokenExpirationMinutes { get; init; } = 30;
    /// <summary>
    /// Angular parola sıfırlama ekranının token parametresi eklenmeden önceki URL adresidir.
    /// </summary>
    public string ResetUrlBase { get; init; } = string.Empty;
}
