using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>
/// Mevcut parolayı doğrular, yeni parolayı kaydeder, SecurityStamp'i yeniler
/// ve bütün refresh token oturumlarını sonlandırır.
/// </summary>
public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IKLMNDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly TimeProvider _timeProvider;

    public ChangePasswordCommandHandler(
        IKLMNDbContext dbContext,
        ICurrentUserService currentUserService,
        IPasswordHasherService passwordHasherService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _passwordHasherService = passwordHasherService;
        _timeProvider = timeProvider;
    }

    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new AuthenticationRequiredException();

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AuthenticationRequiredException();

        if (!_passwordHasherService.VerifyPassword(
                user.PasswordHash,
                request.CurrentPassword))
        {
            throw new InvalidCurrentPasswordException();
        }

        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        user.PasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
        user.PasswordChangedDate = utcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");

        var refreshTokens = await _dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .Where(x => x.UserId == userId && !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.RevokedAt = utcNow;
            token.RevokedByIp = _currentUserService.IpAddress;
            token.RevocationReason = "Password changed.";
            token.IsActive = false;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
