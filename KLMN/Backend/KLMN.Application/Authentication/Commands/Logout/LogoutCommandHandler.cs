using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.Logout;

/// <summary>Mevcut refresh token'ı idempotent şekilde revoke eder.</summary>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IKLMNDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

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

    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return;
        }

        var tokenHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);

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
