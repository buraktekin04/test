namespace KLMN.Infrastructure.Communication;

/// <summary>SMTP sunucu, port, hesap, parola ve gönderen bilgilerini etkin ortamın appsettings.json / appsettings.Development.json dosyasındaki Smtp bölümünden eşleyen modeldir; ayrı bir settings dosyası oluşturmaz.</summary>
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
    /// SMTP sunucusu parola gerektiriyorsa etkin ortamın Smtp:Password alanına yazılan paroladır.
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
