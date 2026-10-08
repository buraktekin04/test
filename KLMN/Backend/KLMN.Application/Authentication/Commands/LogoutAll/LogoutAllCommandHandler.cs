using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.LogoutAll;

/// <summary>SecurityStamp'i yeniler ve bütün refresh token'ları revoke eder.</summary>
public sealed class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand>
{
    private readonly IKLMNDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public LogoutAllCommandHandler(
        IKLMNDbContext dbContext,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async Task Handle(
        LogoutAllCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new AuthenticationRequiredException();

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AuthenticationRequiredException();

        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        user.SecurityStamp = Guid.NewGuid().ToString("N");

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
