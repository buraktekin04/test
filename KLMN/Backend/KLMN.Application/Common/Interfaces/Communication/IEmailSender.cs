namespace KLMN.Application.Common.Interfaces.Communication;

/// <summary>E-posta gönderim altyapısını soyutlar.</summary>
public interface IEmailSender
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
