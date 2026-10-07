namespace KLMN.Application.Common.Settings;

/// <summary>
/// Parola sıfırlama akışı ayarlarını tanımlar.
/// </summary>
public sealed class PasswordResetSettings
{
    public const string SectionName = "PasswordReset";

    public int TokenExpirationMinutes { get; set; } = 30;

    public string ResetUrlBase { get; set; } =
        "http://localhost:4200/reset-password";
}
