using KLMN.Application.Common.Interfaces.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace KLMN.Infrastructure.Email;

/// <summary>
/// MailKit kullanarak SMTP üzerinden HTML e-posta gönderir.
/// </summary>
internal sealed class SmtpEmailSender(
    IOptions<SmtpSettings> smtpOptions)
    : IEmailSender
{
    private readonly SmtpSettings _settings =
        smtpOptions.Value;

    /// <inheritdoc />
    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromAddress));

        message.To.Add(
            MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body =
            new BodyBuilder
            {
                HtmlBody = htmlBody
            }
            .ToMessageBody();

        using var client =
            new SmtpClient();

        var secureSocketOption =
            _settings.UseSsl
                ? SecureSocketOptions.StartTlsWhenAvailable
                : SecureSocketOptions.None;

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            secureSocketOption,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(
                _settings.UserName))
        {
            await client.AuthenticateAsync(
                _settings.UserName,
                _settings.Password,
                cancellationToken);
        }

        await client.SendAsync(
            message,
            cancellationToken);

        await client.DisconnectAsync(
            true,
            cancellationToken);
    }
}
