namespace KLMN.Application.Common.Options;

/// <summary>Parola sıfırlama token süre ve istemci yönlendirme ayarlarını temsil eder.</summary>
public sealed class PasswordResetSettings
{
    public const string SectionName = "PasswordReset";
    public int TokenExpirationMinutes { get; init; } = 30;
    public string ResetUrlBase { get; init; } = string.Empty;
}
