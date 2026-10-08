using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.LogoutAll;

/// <summary>SecurityStamp'i yeniler ve bütün refresh token'ları revoke eder.</summary>
public sealed class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand>
{
    /// <summary>
    /// Kullanıcı, yetki ve oturum verilerine erişen EF Core context sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// İsteği gerçekleştiren kullanıcının kimliğini ve istemci bilgilerini sağlar.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;
    /// <summary>
    /// UTC saatini test edilebilir biçimde sağlayan zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// logout all command handler işlemini uygulamanın ilgili kurallarına göre gerçekleştirir.
    /// </summary>
    public LogoutAllCommandHandler(
        IKLMNDbContext dbContext,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Güvenlik damgasını yenileyerek kullanıcının tüm cihaz oturumlarını geçersiz kılar.
    /// </summary>
    public async Task Handle(
        LogoutAllCommand request,
        CancellationToken cancellationToken)
    {
        // Kimliği doğrulanmış veya hedef kullanıcının benzersiz kimliğidir.
        var userId = _currentUserService.UserId
            ?? throw new AuthenticationRequiredException();

        // İşlem yapılacak kullanıcı hesabının takip edilen EF Core kaydıdır.
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AuthenticationRequiredException();

        // İşlem sırasında tüm tarih karşılaştırmalarında kullanılacak UTC zamanıdır.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        user.SecurityStamp = Guid.NewGuid().ToString("N");

        // refresh tokens değerini mevcut işlemin sonraki kontrollerinde kullanmak üzere hesaplar.
        var refreshTokens = await _dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .Where(x => x.UserId == userId && !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.RevokedAt = utcNow;
            token.RevokedByIp = _currentUserService.IpAddress;
            token.RevocationReason = "Logout from all devices.";
            token.IsActive = false;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
