using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.Logout;

/// <summary>Mevcut refresh token'ı idempotent şekilde revoke eder.</summary>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    /// <summary>
    /// Kullanıcı, yetki ve oturum verilerine erişen EF Core context sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// İmzalı erişim tokenı ve rastgele refresh token üretme hizmetidir.
    /// </summary>
    private readonly IJwtTokenService _jwtTokenService;
    /// <summary>
    /// İsteği gerçekleştiren kullanıcının kimliğini ve istemci bilgilerini sağlar.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;
    /// <summary>
    /// UTC saatini test edilebilir biçimde sağlayan zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// logout command handler işlemini uygulamanın ilgili kurallarına göre gerçekleştirir.
    /// </summary>
    public LogoutCommandHandler(
        IKLMNDbContext dbContext,
        IJwtTokenService jwtTokenService,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Mevcut cihazdaki refresh token oturumunu iptal eder.
    /// </summary>
    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return;
        }

        // Açık tokenın veritabanı ile karşılaştırılacak tek yönlü SHA-256 özetidir.
        var tokenHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);

        // Yeni oturum için üretilen rastgele refresh token ve hash bilgileridir.
        var refreshToken = await _dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);

        if (refreshToken is null || refreshToken.RevokedAt.HasValue)
        {
            return;
        }

        refreshToken.RevokedAt = _timeProvider.GetUtcNow().UtcDateTime;
        refreshToken.RevokedByIp = _currentUserService.IpAddress;
        refreshToken.RevocationReason = "User logout.";
        refreshToken.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
