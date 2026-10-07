namespace KLMN.Application.Common.Settings;

/// <summary>
/// Login başarısızlık ve hesap kilitleme ayarlarını tanımlar.
/// </summary>
public sealed class AuthenticationSettings
{
    public const string SectionName = "Authentication";

    public int MaxFailedAccessAttempts { get; set; } = 5;

    public int LockoutMinutes { get; set; } = 15;
}
