using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>
/// Authenticated kullanıcının parolasını değiştirir.
/// </summary>
internal sealed class ChangePasswordCommandHandler(
    IKLMNDbContext dbContext,
    ICurrentUserService currentUserService,
    IPasswordHasherService passwordHasherService)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not Guid userId)
        {
            throw new AuthenticationRequiredException();
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken)
            ?? throw new AuthenticationRequiredException();

        if (!passwordHasherService.VerifyPassword(
                user.PasswordHash,
                request.CurrentPassword))
        {
            throw new InvalidCurrentPasswordException();
        }

        var now = DateTime.UtcNow;

        user.PasswordHash =
            passwordHasherService.HashPassword(
                request.NewPassword);

        user.PasswordChangedDate = now;
        user.SecurityStamp =
            Guid.NewGuid().ToString("N");

        var activeTokens = await dbContext.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
            token.RevokedByIp = currentUserService.IpAddress;
            token.RevocationReason = "Password changed";
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
