using KLMN.Application.Authentication.Models;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.RefreshToken;

/// <summary>
/// Refresh token rotation ve reuse detection işlemlerini gerçekleştirir.
/// </summary>
internal sealed class RefreshTokenCommandHandler(
    IKLMNDbContext dbContext,
    IJwtTokenService jwtTokenService,
    IUserAuthorizationService userAuthorizationService)
    : IRequestHandler<RefreshTokenCommand, AuthSessionResult>
{
    public async Task<AuthSessionResult> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var tokenHash =
            jwtTokenService.HashRefreshToken(
                request.RefreshToken);

        var storedToken = await dbContext.RefreshTokens
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
                storedToken.ReplacedByTokenId,
                now,
                request.IpAddress,
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            throw new InvalidRefreshTokenException();
        }

        if (storedToken.ExpiresAt <= now)
        {
            throw new InvalidRefreshTokenException();
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Id == storedToken.UserId,
                cancellationToken)
            ?? throw new InvalidRefreshTokenException();

        if (user.IsLocked &&
            (!user.LockoutEnd.HasValue ||
             user.LockoutEnd.Value > now))
        {
            throw new AccountLockedException();
        }

        if ((user.ValidFrom.HasValue &&
             user.ValidFrom.Value > now) ||
            (user.ValidTo.HasValue &&
             user.ValidTo.Value < now))
        {
            throw new InvalidRefreshTokenException();
        }

        var authorization =
            await userAuthorizationService.GetSnapshotAsync(
                user.Id,
                cancellationToken);

        var accessToken =
            jwtTokenService.GenerateAccessToken(
                user,
                authorization.Roles,
                authorization.Permissions);

        var newRefreshToken =
            jwtTokenService.GenerateRefreshToken();

        var replacement =
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshToken.TokenHash,
                ExpiresAt = newRefreshToken.ExpiresAt,
                CreatedByIp = request.IpAddress,
                UserAgent = request.UserAgent,
                DeviceName = request.DeviceName
            };

        dbContext.RefreshTokens.Add(replacement);

        storedToken.RevokedAt = now;
        storedToken.RevokedByIp = request.IpAddress;
        storedToken.RevocationReason = "Refresh token rotation";
        storedToken.ReplacedByTokenId = replacement.Id;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthSessionResult
        {
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
            Response = new AuthSessionResponse
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                User = new AuthUserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email,
                    OrganizationUnitId = user.OrganizationUnitId,
                    Roles = authorization.Roles,
                    Permissions = authorization.Permissions
                }
            }
        };
    }

    private async Task RevokeReplacementChainAsync(
        Guid? replacementTokenId,
        DateTime now,
        string? revokedByIp,
        CancellationToken cancellationToken)
    {
        var currentId = replacementTokenId;

        while (currentId.HasValue)
        {
            var token = await dbContext.RefreshTokens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.Id == currentId.Value,
                    cancellationToken);

            if (token is null)
            {
                break;
            }

            if (!token.RevokedAt.HasValue)
            {
                token.RevokedAt = now;
                token.RevokedByIp = revokedByIp;
                token.RevocationReason =
                    "Refresh token reuse detected";
            }

            currentId = token.ReplacedByTokenId;
        }
    }
}
