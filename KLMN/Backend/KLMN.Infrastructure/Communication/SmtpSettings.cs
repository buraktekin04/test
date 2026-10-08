namespace KLMN.Infrastructure.Communication;

/// <summary>SMTP bağlantı ve gönderen hesap ayarlarını temsil eder.</summary>
public sealed class SmtpSettings
{
    public const string SectionName = "Smtp";
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string FromAddress { get; init; } = string.Empty;
    public string FromName { get; init; } = "KLMN";
}
