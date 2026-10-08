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
    /// <summary>
    /// Kullanıcı, yetki ve oturum verilerine erişen EF Core context sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// Tek kullanımlık parola sıfırlama tokenı oluşturur ve hashler.
    /// </summary>
    private readonly IPasswordResetTokenService _tokenService;
    /// <summary>
    /// Sıfırlama bağlantısının e-posta yoluyla gönderilmesini sağlar.
    /// </summary>
    private readonly IEmailSender _emailSender;
    /// <summary>
    /// İlgili uygulama davranışını yöneten doğrulanmış yapılandırma değerleridir.
    /// </summary>
    private readonly PasswordResetSettings _settings;
    /// <summary>
    /// UTC saatini test edilebilir biçimde sağlayan zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// forgot password command handler işlemini uygulamanın ilgili kurallarına göre gerçekleştirir.
    /// </summary>
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

    /// <summary>
    /// Hesabın varlığını açığa çıkarmadan tek kullanımlık sıfırlama bağlantısı üretir.
    /// </summary>
    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // E-posta hesabını büyük/küçük harften bağımsız aramak için normalize edilmiş değerdir.
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        // İşlem yapılacak kullanıcı hesabının takip edilen EF Core kaydıdır.
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(
                x => x.NormalizedEmail == normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            return;
        }

        // İşlem sırasında tüm tarih karşılaştırmalarında kullanılacak UTC zamanıdır.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        // Yeni sıfırlama bağlantısı öncesi iptal edilmesi gereken eski token kayıtlarıdır.
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

        // Parola sıfırlama için üretilen kısa ömürlü açık token ve hash değeridir.
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

        // Angular parola sıfırlama ekranına giden tek kullanımlık bağlantıdır.
        var resetUrl = BuildResetUrl(generatedToken.Token);
        // Kullanıcıya iletilecek HTML e-posta içerik metnidir.
        var body = BuildEmailBody(user.FirstName, resetUrl, generatedToken.ExpiresAt);

        await _emailSender.SendAsync(
            user.Email,
            "KLMN Parola Sıfırlama",
            body,
            cancellationToken);
    }

    /// <summary>
    /// build reset url işlemini uygulamanın ilgili kurallarına göre gerçekleştirir.
    /// </summary>
    private string BuildResetUrl(string token)
    {
        // Bağlantıda query parametresinin nasıl ekleneceğini belirleyen ayırıcıdır.
        var separator = _settings.ResetUrlBase.Contains('?') ? "&" : "?";
        return $"{_settings.ResetUrlBase}{separator}token={Uri.EscapeDataString(token)}";
    }

    /// <summary>
    /// build email body işlemini uygulamanın ilgili kurallarına göre gerçekleştirir.
    /// </summary>
    private static string BuildEmailBody(
        string firstName,
        string resetUrl,
        DateTime expiresAt)
    {
        // HTML içerisine güvenle yazılabilmesi için escape edilmiş kullanıcı adıdır.
        var safeName = WebUtility.HtmlEncode(firstName);
        // E-posta HTML'inde kullanılmadan önce encode edilmiş bağlantıdır.
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
