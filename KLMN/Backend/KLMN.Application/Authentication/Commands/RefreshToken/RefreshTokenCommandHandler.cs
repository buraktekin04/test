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
    private readonly IKLMNDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

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

    public async Task<RefreshResult> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var tokenHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);

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

        var authorization = await _authorizationService.GetAsync(
            user.Id,
            cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user,
            authorization.Roles,
            authorization.Permissions);

        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

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

    private async Task RevokeReplacementChainAsync(
        KLMN.Domain.Entities.Identity.RefreshToken token,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        var nextTokenId = token.ReplacedByTokenId;
        var visitedIds = new HashSet<Guid>();

        while (nextTokenId.HasValue && visitedIds.Add(nextTokenId.Value))
        {
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
