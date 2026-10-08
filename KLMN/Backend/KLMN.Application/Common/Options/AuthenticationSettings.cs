namespace KLMN.Application.Common.Options;

/// <summary>Kullanıcı giriş güvenliği ve hesap kilitleme ayarlarını temsil eder.</summary>
public sealed class AuthenticationSettings
{
    public const string SectionName = "Authentication";
    public int MaxFailedAccessAttempts { get; init; } = 5;
    public int LockoutMinutes { get; init; } = 15;
}
