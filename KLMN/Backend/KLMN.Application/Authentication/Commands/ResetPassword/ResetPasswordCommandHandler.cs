using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.ResetPassword;

/// <summary>
/// Reset token'ı doğrular, parolayı değiştirir ve mevcut authentication
/// oturumlarını geçersiz hale getirir.
/// </summary>
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IKLMNDbContext _dbContext;
    private readonly IPasswordResetTokenService _tokenService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly TimeProvider _timeProvider;

    public ResetPasswordCommandHandler(
        IKLMNDbContext dbContext,
        IPasswordResetTokenService tokenService,
        IPasswordHasherService passwordHasherService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _passwordHasherService = passwordHasherService;
        _timeProvider = timeProvider;
    }

    public async Task Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var tokenHash = _tokenService.HashToken(request.Token);

        var resetToken = await _dbContext.PasswordResetTokens
            .IgnoreQueryFilters()
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (resetToken is null ||
            resetToken.IsDeleted ||
            !resetToken.IsActive ||
            resetToken.UsedAt.HasValue ||
            resetToken.RevokedAt.HasValue ||
            resetToken.ExpiresAt <= utcNow)
        {
            throw new InvalidPasswordResetTokenException();
        }

        var user = resetToken.User;

        if (user.IsDeleted || !user.IsActive)
        {
            throw new InvalidPasswordResetTokenException();
        }

        user.PasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
        user.PasswordChangedDate = utcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.AccessFailedCount = 0;

        if (user.IsLocked && user.LockoutEnd.HasValue)
        {
            user.IsLocked = false;
            user.LockoutEnd = null;
        }

        resetToken.UsedAt = utcNow;
        resetToken.IsActive = false;

        var otherResetTokens = await _dbContext.PasswordResetTokens
            .IgnoreQueryFilters()
            .Where(x =>
                x.UserId == user.Id &&
                x.Id != resetToken.Id &&
                !x.UsedAt.HasValue &&
                !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in otherResetTokens)
        {
            token.RevokedAt = utcNow;
            token.IsActive = false;
        }

        var refreshTokens = await _dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .Where(x => x.UserId == user.Id && !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.RevokedAt = utcNow;
            token.RevocationReason = "Password reset.";
            token.IsActive = false;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
