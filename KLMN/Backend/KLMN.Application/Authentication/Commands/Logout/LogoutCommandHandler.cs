using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.Logout;

/// <summary>
/// Mevcut refresh token'ı revoke eder.
/// </summary>
internal sealed class LogoutCommandHandler(
    IKLMNDbContext dbContext,
    IJwtTokenService jwtTokenService)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return;
        }

        var hash =
            jwtTokenService.HashRefreshToken(
                request.RefreshToken);

        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == hash,
                cancellationToken);

        if (token is null ||
            token.RevokedAt.HasValue)
        {
            return;
        }

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = request.IpAddress;
        token.RevocationReason = "User logout";

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
