using System.Net;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Communication;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Options;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KLMN.Application.Authentication.Commands.ForgotPassword;

/// <summary>
/// Kullanıcı varlığını dışarı sızdırmadan reset token üretir ve e-posta gönderir.
/// </summary>
public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IKLMNDbContext _dbContext;
    private readonly IPasswordResetTokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly PasswordResetSettings _settings;
    private readonly TimeProvider _timeProvider;

    public ForgotPasswordCommandHandler(
        IKLMNDbContext dbContext,
        IPasswordResetTokenService tokenService,
        IEmailSender emailSender,
        IOptions<PasswordResetSettings> settings,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _settings = settings.Value;
        _timeProvider = timeProvider;
    }

    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(
                x => x.NormalizedEmail == normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            return;
        }

        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        var previousTokens = await _dbContext.PasswordResetTokens
            .IgnoreQueryFilters()
            .Where(x =>
                x.UserId == user.Id &&
                !x.UsedAt.HasValue &&
                !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in previousTokens)
        {
            token.RevokedAt = utcNow;
            token.IsActive = false;
        }

        var generatedToken = _tokenService.GenerateToken();

        await _dbContext.PasswordResetTokens.AddAsync(
            new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = generatedToken.TokenHash,
                ExpiresAt = generatedToken.ExpiresAt
            },
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var resetUrl = BuildResetUrl(generatedToken.Token);
        var body = BuildEmailBody(user.FirstName, resetUrl, generatedToken.ExpiresAt);

        await _emailSender.SendAsync(
            user.Email,
            "KLMN Parola Sıfırlama",
            body,
            cancellationToken);
    }

    private string BuildResetUrl(string token)
    {
        var separator = _settings.ResetUrlBase.Contains('?') ? "&" : "?";
        return $"{_settings.ResetUrlBase}{separator}token={Uri.EscapeDataString(token)}";
    }

    private static string BuildEmailBody(
        string firstName,
        string resetUrl,
        DateTime expiresAt)
    {
        var safeName = WebUtility.HtmlEncode(firstName);
        var safeUrl = WebUtility.HtmlEncode(resetUrl);

        return $"""
                <p>Merhaba {safeName},</p>
                <p>KLMN hesabınız için parola sıfırlama talebi alındı.</p>
                <p><a href="{safeUrl}">Parolamı Sıfırla</a></p>
                <p>Bu bağlantı {expiresAt:dd.MM.yyyy HH:mm} UTC tarihine kadar geçerlidir.</p>
                <p>Bu talebi siz oluşturmadıysanız herhangi bir işlem yapmanız gerekmez.</p>
                """;
    }
}
