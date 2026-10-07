namespace KLMN.Infrastructure.Email;

/// <summary>
/// SMTP e-posta gönderim ayarlarını tanımlar.
/// </summary>
public sealed class SmtpSettings
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FromAddress { get; set; } = string.Empty;

    public string FromName { get; set; } = "KLMN";

    public bool UseSsl { get; set; } = true;
}
