using System.Security.Claims;
using System.Text;
using KLMN.Application.Common.Constants;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Communication;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Options;
using KLMN.Application.Common.Exceptions;
using KLMN.Infrastructure.Authentication;
using KLMN.Infrastructure.Authorization;
using KLMN.Infrastructure.Communication;
using KLMN.Infrastructure.Identity;
using KLMN.Infrastructure.Storage;
using KLMN.Application.Common.Interfaces.Storage;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace KLMN.Infrastructure;

/// <summary>Infrastructure authentication, authorization ve teknik servis kayıtlarını içerir.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Kimlik, JWT, SMTP ve altyapı servislerini DI container'a kaydeder.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        // Mesai denetimi uygulama boyunca aynı hizmetten yapılır.
        services.AddScoped<IWorkingHoursAccessService, WorkingHoursAccessService>();

        // PDF byte'larını veritabanı dışında tutan private dosya deposudur.
        services.AddSingleton<IUserAgreementPdfStorage, LocalUserAgreementPdfStorage>();

        /*
         * Jwt, Authentication, PasswordReset ve Smtp ayarları
         * seçili ortamın JSON konfigürasyonundan IOptions<T>
         * modellerine bağlanır:
         *
         * Production: appsettings.json
         * Development: appsettings.Development.json (öncelikli)
         *
         * Settings/Options sınıfları ayrı ayar deposu değildir; tipli
         * erişim ve başlangıç doğrulaması için kullanılan modellerdir.
         */
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(x => !string.IsNullOrWhiteSpace(x.SecretKey), "Jwt:SecretKey boş bırakılamaz.")
            .Validate(x => Encoding.UTF8.GetByteCount(x.SecretKey) >= 32, "Jwt:SecretKey en az 32 byte olmalıdır.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Issuer), "Jwt:Issuer boş bırakılamaz.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Audience), "Jwt:Audience boş bırakılamaz.")
            .Validate(x => x.AccessTokenExpirationMinutes > 0, "Access token süresi geçersizdir.")
            .Validate(x => x.RefreshTokenExpirationDays > 0, "Refresh token süresi geçersizdir.")
            .ValidateOnStart();

        services
            .AddOptions<AuthenticationSettings>()
            .Bind(configuration.GetSection(AuthenticationSettings.SectionName))
            .Validate(x => x.MaxFailedAccessAttempts > 0, "Authentication:MaxFailedAccessAttempts sıfırdan büyük olmalıdır.")
            .Validate(x => x.LockoutMinutes > 0, "Authentication:LockoutMinutes sıfırdan büyük olmalıdır.")
            .Validate(
                x => !string.IsNullOrWhiteSpace(x.RefreshCookieName) &&
                     !x.RefreshCookieName.Any(char.IsWhiteSpace) &&
                     !x.RefreshCookieName.Contains(';'),
                "Authentication:RefreshCookieName geçerli bir cookie adı olmalıdır.")
            .Validate(
                x => Enum.TryParse<Microsoft.AspNetCore.Http.SameSiteMode>(
                    x.RefreshCookieSameSite, ignoreCase: true, out var mode) &&
                    mode is Microsoft.AspNetCore.Http.SameSiteMode.Lax
                        or Microsoft.AspNetCore.Http.SameSiteMode.Strict
                        or Microsoft.AspNetCore.Http.SameSiteMode.None,
                "Authentication:RefreshCookieSameSite Lax, Strict veya None olmalıdır.")
            .ValidateOnStart();

        services
            .AddOptions<PasswordResetSettings>()
            .Bind(configuration.GetSection(PasswordResetSettings.SectionName))
            .Validate(x => x.TokenExpirationMinutes > 0, "PasswordReset:TokenExpirationMinutes sıfırdan büyük olmalıdır.")
            .Validate(
                x => Uri.TryCreate(x.ResetUrlBase, UriKind.Absolute, out _),
                "PasswordReset:ResetUrlBase geçerli bir URL olmalıdır.")
            .ValidateOnStart();

        services
            .AddOptions<SmtpSettings>()
            .Bind(configuration.GetSection(SmtpSettings.SectionName))
            .Validate(x => !string.IsNullOrWhiteSpace(x.Host), "Smtp:Host zorunludur.")
            .Validate(x => x.Port > 0, "Smtp:Port geçersizdir.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.FromAddress), "Smtp:FromAddress zorunludur.")
            .ValidateOnStart();

        /*
         * Mesai saatleri appsettings içindeki WorkingHours bölümünden okunur.
         * Başlangıç dahil, bitiş hariç ve ISO günleri 1..7 biçimindedir.
         */
        services.AddOptions<WorkingHoursSettings>()
            .Bind(configuration.GetSection(WorkingHoursSettings.SectionName))
            .Validate(x =>
                !string.IsNullOrWhiteSpace(x.TimeZoneId) &&
                IsValidTimeZone(x.TimeZoneId),
                "WorkingHours:TimeZoneId geçerli bir saat dilimi olmalıdır.")
            .Validate(x =>
                TimeOnly.TryParseExact(x.Start, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) &&
                TimeOnly.TryParseExact(x.End, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var end) &&
                start < end,
                "WorkingHours:Start ve End HH:mm formatında olmalı, Start < End olmalıdır.")
            .Validate(x =>
                x.DaysOfWeek is { Length: > 0 } &&
                x.DaysOfWeek.All(day => day is >= 1 and <= 7),
                "WorkingHours:DaysOfWeek 1..7 arasında günler içermelidir.")
            .ValidateOnStart();

        // Kök klasör yolu ve maksimum yükleme boyutu her ortamda farklıdır.
        services.AddOptions<DocumentStorageSettings>()
            .Bind(configuration.GetSection(DocumentStorageSettings.SectionName))
            .Validate(x => !string.IsNullOrWhiteSpace(x.RootPath),
                "DocumentStorage:RootPath zorunludur.")
            .Validate(x => x.MaxFileSizeMb is > 0 and <= 50,
                "DocumentStorage:MaxFileSizeMb 1-50 arasında olmalıdır.")
            .ValidateOnStart();

        // Jwt bölümünden okunan imza, issuer, audience ve ömür değerleri Bearer doğrulama kurallarını oluşturur.
        var jwtSettings =
            configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT ayarları bulunamadı.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };

                options.Events =
                    new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            // Token mesai nedeni ile reddedildiyse 401 yerine açıklayıcı 403 döndür.
                            if (context.HttpContext.Items["OutsideWorkingHours"] is true)
                            {
                                context.HandleResponse();
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                context.Response.ContentType = "application/problem+json";
                                await context.Response.WriteAsJsonAsync(
                                    new Microsoft.AspNetCore.Mvc.ProblemDetails
                                    {
                                        Status = StatusCodes.Status403Forbidden,
                                        Title = "Mesai dışı erişim engellendi.",
                                        Detail = new OutsideWorkingHoursException().Message
                                    });
                            }
                        },

                        OnAuthenticationFailed = context =>
                        {
                            // logger bilgisini sonraki işlem adımları için hesaplar.
                            var logger = context.HttpContext
                                .RequestServices
                                .GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                                .CreateLogger("JwtAuthentication");

                            logger.LogWarning(
                                context.Exception,
                                "JWT authentication başarısız. Path: {Path}",
                                context.HttpContext.Request.Path);

                            return Task.CompletedTask;
                        },

                        OnTokenValidated = async context =>
                        {
                            // user id value bilgisini sonraki işlem adımları için hesaplar.
                            var userIdValue =
                                context.Principal?
                                    .FindFirstValue(ClaimTypes.NameIdentifier);

                            // token security stamp bilgisini sonraki işlem adımları için hesaplar.
                            var tokenSecurityStamp =
                                context.Principal?
                                    .FindFirstValue(CustomClaimTypes.SecurityStamp);

                            if (!Guid.TryParse(userIdValue, out var userId) ||
                                string.IsNullOrWhiteSpace(tokenSecurityStamp))
                            {
                                context.Fail("Geçersiz kullanıcı kimliği veya security stamp.");
                                return;
                            }

                            // db context bilgisini sonraki işlem adımları için hesaplar.
                            var dbContext = context.HttpContext
                                .RequestServices
                                .GetRequiredService<IKLMNDbContext>();

                            // time provider bilgisini sonraki işlem adımları için hesaplar.
                            var timeProvider = context.HttpContext
                                .RequestServices
                                .GetRequiredService<TimeProvider>();

                            // Token bitiş zamanlarını UTC üzerinden hesaplamak için güncel zamandır.
                            var utcNow = timeProvider.GetUtcNow().UtcDateTime;

                            // user bilgisini sonraki işlem adımları için hesaplar.
                            var user = await dbContext.Users
                                .AsNoTracking()
                                .Where(x => x.Id == userId)
                                .Select(x => new
                                {
                                    x.SecurityStamp,
                                    x.IsLocked,
                                    x.LockoutEnd,
                                    x.ValidFrom,
                                    x.ValidTo,
                                    x.ClockLimit
                                })
                                .SingleOrDefaultAsync(
                                    context.HttpContext.RequestAborted);

                            if (user is null)
                            {
                                context.Fail("Kullanıcı artık aktif değildir.");
                                return;
                            }

                            if (!string.Equals(
                                    user.SecurityStamp,
                                    tokenSecurityStamp,
                                    StringComparison.Ordinal))
                            {
                                context.Fail("Oturum artık geçerli değildir.");
                                return;
                            }

                            if (user.IsLocked &&
                                (!user.LockoutEnd.HasValue ||
                                 user.LockoutEnd.Value > utcNow))
                            {
                                context.Fail("Kullanıcı hesabı kilitlidir.");
                                return;
                            }

                            if (user.ValidFrom.HasValue &&
                                user.ValidFrom.Value > utcNow)
                            {
                                context.Fail("Kullanıcı hesabı henüz geçerli değildir.");
                                return;
                            }

                            if (user.ValidTo.HasValue &&
                                user.ValidTo.Value <= utcNow)
                            {
                                context.Fail("Kullanıcı hesabının geçerlilik süresi dolmuştur.");
                                return;
                            }

                            /*
                             * Süresi henüz dolmamış JWT bile mesai sona erince
                             * erişim veremez. Kullanıcı kısıtı DB'den anlık okunur.
                             */
                            var workingHours = context.HttpContext.RequestServices
                                .GetRequiredService<IWorkingHoursAccessService>();

                            if (!workingHours.IsAllowed(user.ClockLimit))
                            {
                                context.HttpContext.Items["OutsideWorkingHours"] = true;
                                context.Fail("Mesai dışı erişim engellendi.");
                            }
                        }
                    };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy =
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
        });

        services.AddSingleton<
            IAuthorizationPolicyProvider,
            PermissionAuthorizationPolicyProvider>();

        services.AddScoped<
            IAuthorizationHandler,
            PermissionAuthorizationHandler>();

        return services;
    }

    /// <summary>Çalışma saati ayarındaki IANA/Windows timezone kodunu kontrol eder.</summary>
    private static bool IsValidTimeZone(string value)
    {
        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(value);
            return true;
        }
        catch (TimeZoneNotFoundException) { return false; }
        catch (InvalidTimeZoneException) { return false; }
    }
}
