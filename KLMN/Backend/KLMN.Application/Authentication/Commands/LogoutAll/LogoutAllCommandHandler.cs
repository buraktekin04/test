using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.LogoutAll;

/// <summary>
/// Tüm refresh token'ları revoke eder ve SecurityStamp değerini yeniler.
/// </summary>
internal sealed class LogoutAllCommandHandler(
    IKLMNDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<LogoutAllCommand>
{
    public async Task Handle(
        LogoutAllCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not Guid userId)
        {
            throw new AuthenticationRequiredException();
        }

        var now = DateTime.UtcNow;

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken)
            ?? throw new AuthenticationRequiredException();

        var activeTokens = await dbContext.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
            token.RevokedByIp = currentUserService.IpAddress;
            token.RevocationReason = "Logout all";
        }

        user.SecurityStamp =
            Guid.NewGuid().ToString("N");

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
