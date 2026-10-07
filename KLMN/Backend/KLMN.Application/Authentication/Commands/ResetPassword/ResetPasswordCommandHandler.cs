using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.ResetPassword;

/// <summary>
/// Geçerli reset token ile parolayı değiştirir ve mevcut session'ları sonlandırır.
/// </summary>
internal sealed class ResetPasswordCommandHandler(
    IKLMNDbContext dbContext,
    IPasswordResetTokenService passwordResetTokenService,
    IPasswordHasherService passwordHasherService)
    : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var tokenHash =
            passwordResetTokenService.HashToken(
                request.Token);

        var resetToken = await dbContext.PasswordResetTokens
            .FirstOrDefaultAsync(
                x =>
                    x.TokenHash == tokenHash &&
                    x.UsedAt == null &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > now,
                cancellationToken)
            ?? throw new InvalidPasswordResetTokenException();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Id == resetToken.UserId,
                cancellationToken)
            ?? throw new InvalidPasswordResetTokenException();

        user.PasswordHash =
            passwordHasherService.HashPassword(
                request.NewPassword);

        user.PasswordChangedDate = now;
        user.SecurityStamp =
            Guid.NewGuid().ToString("N");

        resetToken.UsedAt = now;

        var otherResetTokens =
            await dbContext.PasswordResetTokens
                .Where(x =>
                    x.UserId == user.Id &&
                    x.Id != resetToken.Id &&
                    x.UsedAt == null &&
                    x.RevokedAt == null)
                .ToListAsync(cancellationToken);

        foreach (var token in otherResetTokens)
        {
            token.RevokedAt = now;
        }

        var refreshTokens =
            await dbContext.RefreshTokens
                .Where(x =>
                    x.UserId == user.Id &&
                    x.RevokedAt == null)
                .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.RevokedAt = now;
            refreshToken.RevocationReason =
                "Password reset";
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
