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
    /// <summary>
    /// Kullanıcı, yetki ve oturum verilerine erişen EF Core context sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// Parola karşılaştırma ve güvenli hash üretme hizmetidir.
    /// </summary>
    private readonly IPasswordHasherService _passwordHasherService;
    /// <summary>
    /// İmzalı erişim tokenı ve rastgele refresh token üretme hizmetidir.
    /// </summary>
    private readonly IJwtTokenService _jwtTokenService;
    /// <summary>
    /// Rol ve kullanıcı override kayıtlarından geçerli yetki kümesini hesaplar.
    /// </summary>
    private readonly IUserAuthorizationService _authorizationService;
    /// <summary>
    /// İsteği gerçekleştiren kullanıcının kimliğini ve istemci bilgilerini sağlar.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;
    /// <summary>
    /// Hesap kilitleme eşiği ve sürelerini içeren doğrulanmış ayarlardır.
    /// </summary>
    private readonly AuthenticationSettings _authenticationSettings;
    /// <summary>
    /// UTC saatini test edilebilir biçimde sağlayan zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Giriş sürecine gerekli veritabanı, parola doğrulama, JWT, yetkilendirme ve zaman bağımlılıklarını bağlar.
    /// </summary>
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

    /// <summary>
    /// Kullanıcı girişini doğrular, hesap kilitlerini denetler ve JWT/refresh oturumu oluşturur.
    /// </summary>
    public async Task<LoginResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // İşlem sırasında tüm tarih karşılaştırmalarında kullanılacak UTC zamanıdır.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        // Girişte kullanıcı adı/e-posta için oluşturulmuş normalize arama anahtarıdır.
        var normalizedIdentifier = Normalize(request.Identifier);

        // İşlem yapılacak kullanıcı hesabının takip edilen EF Core kaydıdır.
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

        // Girilen parolanın veritabanındaki hash ile eşleşme sonucudur.
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

        // Kullanıcının güncel rol ve effective permission bilgilerinin özetidir.
        var authorization = await _authorizationService.GetAsync(
            user.Id,
            cancellationToken);

        // Kullanıcının yetkileriyle imzalanan kısa ömürlü erişim tokenı bilgisidir.
        var accessToken = _jwtTokenService.GenerateAccessToken(
            user,
            authorization.Roles,
            authorization.Permissions);

        // Yeni oturum için üretilen rastgele refresh token ve hash bilgileridir.
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

    /// <summary>
    /// Hesabın başlangıç ve bitiş tarihlerini UTC ile karşılaştırır; geçersiz dönemde girişe izin vermez.
    /// </summary>
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

    /// <summary>
    /// Süresi dolan geçici giriş kilidini kaldırır; süresiz yönetici kilitlerine dokunmaz.
    /// </summary>
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

    /// <summary>
    /// Hatalı parola sayacını artırır; eşik aşılırsa geçici kilit koyup güvenlik durumunu veritabanına kaydeder.
    /// </summary>
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

    /// <summary>
    /// Başarılı girişte hatalı deneme sayısını ve geçici hesap kilitlerini sıfırlar.
    /// </summary>
    private static void ResetFailedLoginState(User user)
    {
        user.AccessFailedCount = 0;
        user.IsLocked = false;
        user.LockoutEnd = null;
    }

    /// <summary>
    /// Giriş tanımlayıcısını kullanıcı adı/e-posta aramasına uygun invariant büyük harf formatına dönüştürür.
    /// </summary>
    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();
}
