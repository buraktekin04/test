namespace KLMN.Application.Common.Interfaces.Email;

/// <summary>
/// Uygulamanın e-posta gönderim abstraction'ıdır.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// HTML içerikli e-posta gönderir.
    /// </summary>
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
