using KLMN.Application.Common.Interfaces.Email;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using KLMN.Application.Common.Settings;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KLMN.Application.Authentication.Commands.ForgotPassword;

/// <summary>
/// Kullanıcı varlığını dışarı sızdırmadan parola sıfırlama token'ı üretir.
/// </summary>
internal sealed class ForgotPasswordCommandHandler(
    IKLMNDbContext dbContext,
    IPasswordResetTokenService passwordResetTokenService,
    IEmailSender emailSender,
    IOptions<PasswordResetSettings> passwordResetOptions)
    : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x => x.NormalizedEmail == normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            return;
        }

        var now = DateTime.UtcNow;

        var oldTokens = await dbContext.PasswordResetTokens
            .Where(x =>
                x.UserId == user.Id &&
                x.UsedAt == null &&
                x.RevokedAt == null &&
                x.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        foreach (var oldToken in oldTokens)
        {
            oldToken.RevokedAt = now;
        }

        var generatedToken =
            passwordResetTokenService.GenerateToken();

        dbContext.PasswordResetTokens.Add(
            new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = generatedToken.TokenHash,
                ExpiresAt = generatedToken.ExpiresAt
            });

        await dbContext.SaveChangesAsync(cancellationToken);

        var settings =
            passwordResetOptions.Value;

        var separator =
            settings.ResetUrlBase.Contains('?')
                ? "&"
                : "?";

        var resetUrl =
            $"{settings.ResetUrlBase}{separator}token={Uri.EscapeDataString(generatedToken.Token)}";

        var htmlBody =
            $"""
             <p>Merhaba {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>
             <p>KLMN hesabınız için parola sıfırlama talebi alındı.</p>
             <p><a href="{System.Net.WebUtility.HtmlEncode(resetUrl)}">Parolamı Sıfırla</a></p>
             <p>Bu talebi siz yapmadıysanız bu e-postayı dikkate almayınız.</p>
             """;

        await emailSender.SendAsync(
            user.Email,
            "KLMN Parola Sıfırlama",
            htmlBody,
            cancellationToken);
    }
}
