using System.Security.Claims;
using System.Text;
using KLMN.Application.Common.Constants;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Communication;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Options;
using KLMN.Infrastructure.Authentication;
using KLMN.Infrastructure.Authorization;
using KLMN.Infrastructure.Communication;
using KLMN.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace KLMN.Infrastructure;

/// <summary>Infrastructure authentication, authorization ve teknik servis kayıtlarını içerir.</summary>
public static class DependencyInjection
{
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
                        OnAuthenticationFailed = context =>
                        {
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
                            var userIdValue =
                                context.Principal?
                                    .FindFirstValue(ClaimTypes.NameIdentifier);

                            var tokenSecurityStamp =
                                context.Principal?
                                    .FindFirstValue(CustomClaimTypes.SecurityStamp);

                            if (!Guid.TryParse(userIdValue, out var userId) ||
                                string.IsNullOrWhiteSpace(tokenSecurityStamp))
                            {
                                context.Fail("Geçersiz kullanıcı kimliği veya security stamp.");
                                return;
                            }

                            var dbContext = context.HttpContext
                                .RequestServices
                                .GetRequiredService<IKLMNDbContext>();

                            var timeProvider = context.HttpContext
                                .RequestServices
                                .GetRequiredService<TimeProvider>();

                            var utcNow = timeProvider.GetUtcNow().UtcDateTime;

                            var user = await dbContext.Users
                                .AsNoTracking()
                                .Where(x => x.Id == userId)
                                .Select(x => new
                                {
                                    x.SecurityStamp,
                                    x.IsLocked,
                                    x.LockoutEnd,
                                    x.ValidFrom,
                                    x.ValidTo
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
}
