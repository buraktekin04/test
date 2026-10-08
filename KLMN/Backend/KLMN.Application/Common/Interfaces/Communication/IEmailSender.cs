namespace KLMN.Application.Common.Interfaces.Communication;

/// <summary>E-posta gönderim altyapısını soyutlar.</summary>
public interface IEmailSender
{
    /// <summary>
    /// send async işlemini çağıran katmana belirtilen sözleşmeyle sunar.
    /// </summary>
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
