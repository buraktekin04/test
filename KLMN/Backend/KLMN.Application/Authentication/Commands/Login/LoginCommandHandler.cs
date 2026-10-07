using KLMN.Application.Authentication.Models;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using KLMN.Application.Common.Settings;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>
/// Login işlemini gerçekleştirir.
/// </summary>
internal sealed class LoginCommandHandler(
    IKLMNDbContext dbContext,
    IPasswordHasherService passwordHasherService,
    IJwtTokenService jwtTokenService,
    IUserAuthorizationService userAuthorizationService,
    IOptions<AuthenticationSettings> authenticationOptions)
    : IRequestHandler<LoginCommand, AuthSessionResult>
{
    public async Task<AuthSessionResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var normalized =
            request.UserNameOrEmail
                .Trim()
                .ToUpperInvariant();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x =>
                    x.NormalizedUserName == normalized ||
                    x.NormalizedEmail == normalized,
                cancellationToken);

        if (user is null)
        {
            throw new LoginFailedException();
        }

        if ((user.ValidFrom.HasValue &&
             user.ValidFrom.Value > now) ||
            (user.ValidTo.HasValue &&
             user.ValidTo.Value < now))
        {
            throw new LoginFailedException();
        }

        if (user.IsLocked)
        {
            if (user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value <= now)
            {
                user.IsLocked = false;
                user.LockoutEnd = null;
                user.AccessFailedCount = 0;
            }
            else
            {
                throw new AccountLockedException();
            }
        }

        var passwordValid =
            passwordHasherService.VerifyPassword(
                user.PasswordHash,
                request.Password);

        if (!passwordValid)
        {
            var settings = authenticationOptions.Value;

            user.AccessFailedCount++;

            if (user.AccessFailedCount >=
                settings.MaxFailedAccessAttempts)
            {
                user.IsLocked = true;
                user.LockoutEnd =
                    now.AddMinutes(settings.LockoutMinutes);

                await dbContext.SaveChangesAsync(
                    cancellationToken);

                throw new AccountLockedException();
            }

            await dbContext.SaveChangesAsync(
                cancellationToken);

            throw new LoginFailedException();
        }

        user.AccessFailedCount = 0;
        user.IsLocked = false;
        user.LockoutEnd = null;
        user.LastLoginDate = now;

        var authorization =
            await userAuthorizationService.GetSnapshotAsync(
                user.Id,
                cancellationToken);

        var accessToken =
            jwtTokenService.GenerateAccessToken(
                user,
                authorization.Roles,
                authorization.Permissions);

        var refreshToken =
            jwtTokenService.GenerateRefreshToken();

        dbContext.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshToken.TokenHash,
                ExpiresAt = refreshToken.ExpiresAt,
                CreatedByIp = request.IpAddress,
                UserAgent = request.UserAgent,
                DeviceName = request.DeviceName
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new AuthSessionResult
        {
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            Response = new AuthSessionResponse
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                User = CreateUserResponse(
                    user,
                    authorization.Roles,
                    authorization.Permissions)
            }
        };
    }

    private static AuthUserResponse CreateUserResponse(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        return new AuthUserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Email = user.Email,
            OrganizationUnitId = user.OrganizationUnitId,
            Roles = roles,
            Permissions = permissions
        };
    }
}
