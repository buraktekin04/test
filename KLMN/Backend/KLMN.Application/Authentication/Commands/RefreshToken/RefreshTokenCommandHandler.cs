using KLMN.Application.Authentication.Responses;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.RefreshToken;

/// <summary>
/// Refresh token'ı doğrular, rotation uygular ve reuse durumunda
/// replacement zincirini revoke eder.
/// </summary>
public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, RefreshResult>
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
    /// Rol ve kullanıcı override kayıtlarından geçerli yetki kümesini hesaplar.
    /// </summary>
    private readonly IUserAuthorizationService _authorizationService;
    /// <summary>
    /// İsteği gerçekleştiren kullanıcının kimliğini ve istemci bilgilerini sağlar.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;
    /// <summary>
    /// UTC saatini test edilebilir biçimde sağlayan zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// refresh token command handler işlemini uygulamanın ilgili kurallarına göre gerçekleştirir.
    /// </summary>
    public RefreshTokenCommandHandler(
        IKLMNDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IUserAuthorizationService authorizationService,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Refresh tokenı doğrular ve rotation yaparak yeni token çiftini üretir.
    /// </summary>
    public async Task<RefreshResult> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // İşlem sırasında tüm tarih karşılaştırmalarında kullanılacak UTC zamanıdır.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        // Açık tokenın veritabanı ile karşılaştırılacak tek yönlü SHA-256 özetidir.
        var tokenHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);

        // Gönderilen hash ile eşleşen veritabanı refresh token kaydıdır.
        var storedToken = await _dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);

        if (storedToken is null)
        {
            throw new InvalidRefreshTokenException();
        }

        if (storedToken.RevokedAt.HasValue)
        {
            await RevokeReplacementChainAsync(
                storedToken,
                utcNow,
                cancellationToken);

            throw new InvalidRefreshTokenException();
        }

        if (storedToken.IsDeleted ||
            !storedToken.IsActive ||
            storedToken.ExpiresAt <= utcNow)
        {
            throw new InvalidRefreshTokenException();
        }

        // İşlem yapılacak kullanıcı hesabının takip edilen EF Core kaydıdır.
        var user = storedToken.User;

        if (user.IsDeleted || !user.IsActive)
        {
            throw new InvalidRefreshTokenException();
        }

        if (user.IsLocked)
        {
            if (!user.LockoutEnd.HasValue || user.LockoutEnd.Value > utcNow)
            {
                throw new AccountLockedException(user.LockoutEnd);
            }

            user.IsLocked = false;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
        }

        if ((user.ValidFrom.HasValue && user.ValidFrom.Value > utcNow) ||
            (user.ValidTo.HasValue && user.ValidTo.Value <= utcNow))
        {
            throw new InvalidRefreshTokenException();
        }

        // Kullanıcının güncel rol ve effective permission bilgilerinin özetidir.
        var authorization = await _authorizationService.GetAsync(
            user.Id,
            cancellationToken);

        // Kullanıcının yetkileriyle imzalanan kısa ömürlü erişim tokenı bilgisidir.
        var accessToken = _jwtTokenService.GenerateAccessToken(
            user,
            authorization.Roles,
            authorization.Permissions);

        // Rotation işlemiyle eski oturum tokenının yerine geçen yeni token bilgileridir.
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Rotation sonucu oluşturulan yeni refresh token entity kaydıdır.
        var newEntity = new KLMN.Domain.Entities.Identity.RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshToken.TokenHash,
            ExpiresAt = newRefreshToken.ExpiresAt,
            CreatedByIp = _currentUserService.IpAddress,
            UserAgent = _currentUserService.UserAgent,
            DeviceName = storedToken.DeviceName
        };

        await _dbContext.RefreshTokens.AddAsync(newEntity, cancellationToken);

        storedToken.RevokedAt = utcNow;
        storedToken.RevokedByIp = _currentUserService.IpAddress;
        storedToken.RevocationReason = "Refresh token rotated.";
        storedToken.ReplacedByTokenId = newEntity.Id;
        storedToken.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshResult
        {
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
            Response = new RefreshResponse
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                User = new LoginUserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    OrganizationUnitId = user.OrganizationUnitId,
                    Roles = authorization.Roles,
                    Permissions = authorization.Permissions
                }
            }
        };
    }

    /// <summary>
    /// revoke replacement chain async işlemini asenkron olarak yürütür; iptal isteğini destekler.
    /// </summary>
    private async Task RevokeReplacementChainAsync(
        KLMN.Domain.Entities.Identity.RefreshToken token,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        // next token id değerini mevcut işlemin sonraki kontrollerinde kullanmak üzere hesaplar.
        var nextTokenId = token.ReplacedByTokenId;
        // visited ids değerini mevcut işlemin sonraki kontrollerinde kullanmak üzere hesaplar.
        var visitedIds = new HashSet<Guid>();

        while (nextTokenId.HasValue && visitedIds.Add(nextTokenId.Value))
        {
            // replacement token değerini mevcut işlemin sonraki kontrollerinde kullanmak üzere hesaplar.
            var replacementToken = await _dbContext.RefreshTokens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.Id == nextTokenId.Value,
                    cancellationToken);

            if (replacementToken is null)
            {
                break;
            }

            if (!replacementToken.RevokedAt.HasValue)
            {
                replacementToken.RevokedAt = utcNow;
                replacementToken.RevokedByIp = _currentUserService.IpAddress;
                replacementToken.RevocationReason = "Refresh token reuse detected.";
                replacementToken.IsActive = false;
            }

            nextTokenId = replacementToken.ReplacedByTokenId;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
