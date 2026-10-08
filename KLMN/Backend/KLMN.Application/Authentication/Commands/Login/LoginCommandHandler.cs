using KLMN.Application.Authentication.Responses;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Options;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>
/// Kullanıcı giriş bilgilerini doğrular, hesap güvenliği kontrollerini uygular
/// ve access/refresh token üretir.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IKLMNDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly TimeProvider _timeProvider;

    public LoginCommandHandler(
        IKLMNDbContext dbContext,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService,
        IUserAuthorizationService authorizationService,
        ICurrentUserService currentUserService,
        IOptions<AuthenticationSettings> authenticationSettings,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
        _authenticationSettings = authenticationSettings.Value;
        _timeProvider = timeProvider;
    }

    public async Task<LoginResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var normalizedIdentifier = Normalize(request.Identifier);

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(
                x =>
                    x.NormalizedUserName == normalizedIdentifier ||
                    x.NormalizedEmail == normalizedIdentifier,
                cancellationToken);

        if (user is null)
        {
            throw new LoginFailedException();
        }

        ValidateAccountPeriod(user, utcNow);
        HandleExpiredLockout(user, utcNow);

        if (user.IsLocked)
        {
            throw new AccountLockedException(user.LockoutEnd);
        }

        var passwordValid = _passwordHasherService.VerifyPassword(
            user.PasswordHash,
            request.Password);

        if (!passwordValid)
        {
            await HandleFailedLoginAsync(user, utcNow, cancellationToken);
            throw new LoginFailedException();
        }

        ResetFailedLoginState(user);
        user.LastLoginDate = utcNow;

        var authorization = await _authorizationService.GetAsync(
            user.Id,
            cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user,
            authorization.Roles,
            authorization.Permissions);

        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        await _dbContext.RefreshTokens.AddAsync(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshToken.TokenHash,
                ExpiresAt = refreshToken.ExpiresAt,
                CreatedByIp = _currentUserService.IpAddress,
                UserAgent = _currentUserService.UserAgent,
                DeviceName = string.IsNullOrWhiteSpace(request.DeviceName)
                    ? "Web"
                    : request.DeviceName.Trim()
            },
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResult
        {
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            Response = new LoginResponse
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                User = new LoginUserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    OrganizationUnitId = user.OrganizationUnitId,
                    Roles = authorization.Roles,
                    Permissions = authorization.Permissions
                }
            }
        };
    }

    private static void ValidateAccountPeriod(User user, DateTime utcNow)
    {
        if (user.ValidFrom.HasValue && user.ValidFrom.Value > utcNow)
        {
            throw new LoginFailedException();
        }

        if (user.ValidTo.HasValue && user.ValidTo.Value <= utcNow)
        {
            throw new LoginFailedException();
        }
    }

    private static void HandleExpiredLockout(User user, DateTime utcNow)
    {
        if (!user.IsLocked ||
            !user.LockoutEnd.HasValue ||
            user.LockoutEnd.Value > utcNow)
        {
            return;
        }

        user.IsLocked = false;
        user.LockoutEnd = null;
        user.AccessFailedCount = 0;
    }

    private async Task HandleFailedLoginAsync(
        User user,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;

        if (user.AccessFailedCount >= _authenticationSettings.MaxFailedAccessAttempts)
        {
            user.IsLocked = true;
            user.LockoutEnd = utcNow.AddMinutes(_authenticationSettings.LockoutMinutes);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void ResetFailedLoginState(User user)
    {
        user.AccessFailedCount = 0;
        user.IsLocked = false;
        user.LockoutEnd = null;
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();
}
