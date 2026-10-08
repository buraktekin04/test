namespace KLMN.Infrastructure.Communication;

/// <summary>SMTP bağlantı ve gönderen hesap ayarlarını temsil eder.</summary>
public sealed class SmtpSettings
{
    /// <summary>
    /// section name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public const string SectionName = "Smtp";
    /// <summary>
    /// SMTP sunucu adresidir.
    /// </summary>
    public string Host { get; init; } = string.Empty;
    /// <summary>
    /// SMTP servisinin bağlantı portudur.
    /// </summary>
    public int Port { get; init; } = 587;
    /// <summary>
    /// SMTP sunucusuna kimlik doğrulamada kullanılan hesap adıdır.
    /// </summary>
    public string? UserName { get; init; }
    /// <summary>
    /// SMTP hesap parolasıdır; secrets altyapısında tutulmalıdır.
    /// </summary>
    public string? Password { get; init; }
    /// <summary>
    /// Gönderilen e-postaların From başlığında görülen geçerli adrestir.
    /// </summary>
    public string FromAddress { get; init; } = string.Empty;
    /// <summary>
    /// E-postaların gönderen alanında gösterilecek okunabilir addır.
    /// </summary>
    public string FromName { get; init; } = "KLMN";
}
