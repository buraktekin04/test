using KLMN.Application.Common.Interfaces.Communication;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace KLMN.Infrastructure.Communication;

/// <summary>MailKit kullanarak SMTP üzerinden HTML e-posta gönderir.</summary>
public sealed class SmtpEmailSender : IEmailSender
{
    /// <summary>
    /// JWT veya SMTP güvenlik ve bağlantı ayarlarının doğrulanmış nesnesidir.
    /// </summary>
    private readonly SmtpSettings _settings;

    /// <summary>
    /// smtp email sender işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public SmtpEmailSender(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    /// <summary>
    /// Kullanıcıya MIME HTML e-postasını SMTP üzerinden asenkron gönderir.
    /// </summary>
    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        // SMTP üzerinden gönderilecek MIME e-posta iletisidir.
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromAddress));

        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.Auto,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(_settings.UserName))
        {
            await client.AuthenticateAsync(
                _settings.UserName,
                _settings.Password,
                cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
