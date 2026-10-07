using System.Security.Claims;
using System.Text;
using KLMN.Application.Common.Interfaces.Email;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using KLMN.Domain.Constants;
using KLMN.Infrastructure.Authentication;
using KLMN.Infrastructure.Authorization;
using KLMN.Infrastructure.Email;
using KLMN.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace KLMN.Infrastructure;

/// <summary>
/// Infrastructure katmanının dependency injection kayıtlarını içerir.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Authentication, authorization, security, current-user ve e-posta
    /// servislerini DI container'a ekler.
/// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.Configure<JwtSettings>(
            configuration.GetSection(
                JwtSettings.SectionName));

        services.Configure<SmtpSettings>(
            configuration.GetSection(
                SmtpSettings.SectionName));

        services.AddScoped<
            ICurrentUserService,
            CurrentUserService>();

        services.AddSingleton<
            IPasswordHasherService,
            PasswordHasherService>();

        services.AddSingleton<
            IJwtTokenService,
            JwtTokenService>();

        services.AddSingleton<
            IPasswordResetTokenService,
            PasswordResetTokenService>();

        services.AddScoped<
            IEmailSender,
            SmtpEmailSender>();

        var jwtSettings =
            configuration
                .GetSection(
                    JwtSettings.SectionName)
                .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "Jwt ayarları bulunamadı.");

        if (string.IsNullOrWhiteSpace(
            jwtSettings.SecretKey))
        {
            throw new InvalidOperationException(
                "Jwt:SecretKey yapılandırılmalıdır.");
        }

        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.SecretKey)),

                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateLifetime = true,

                        ClockSkew =
                            TimeSpan.FromSeconds(30),

                        NameClaimType =
                            ClaimTypes.Name,

                        RoleClaimType =
                            ClaimTypes.Role
                    };

                options.Events =
                    new JwtBearerEvents
                    {
                        OnTokenValidated =
                            async context =>
                            {
                                var userIdValue =
                                    context.Principal?
                                        .FindFirstValue(
                                            ClaimTypes.NameIdentifier);

                                var securityStamp =
                                    context.Principal?
                                        .FindFirstValue(
                                            CustomClaimTypes.SecurityStamp);

                                if (!Guid.TryParse(
                                        userIdValue,
                                        out var userId) ||
                                    string.IsNullOrWhiteSpace(
                                        securityStamp))
                                {
                                    context.Fail(
                                        "Token kullanıcı bilgisi geçersiz.");

                                    return;
                                }

                                var dbContext =
                                    context.HttpContext
                                        .RequestServices
                                        .GetRequiredService<
                                            IKLMNDbContext>();

                                var now =
                                    DateTime.UtcNow;

                                var user =
                                    await dbContext.Users
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            x => x.Id == userId,
                                            context.HttpContext
                                                .RequestAborted);

                                if (user is null)
                                {
                                    context.Fail(
                                        "Kullanıcı bulunamadı.");

                                    return;
                                }

                                if (!string.Equals(
                                        user.SecurityStamp,
                                        securityStamp,
                                        StringComparison.Ordinal))
                                {
                                    context.Fail(
                                        "Security stamp geçersiz.");

                                    return;
                                }

                                if (user.IsLocked &&
                                    (!user.LockoutEnd.HasValue ||
                                     user.LockoutEnd.Value > now))
                                {
                                    context.Fail(
                                        "Kullanıcı hesabı kilitli.");

                                    return;
                                }

                                if ((user.ValidFrom.HasValue &&
                                     user.ValidFrom.Value > now) ||
                                    (user.ValidTo.HasValue &&
                                     user.ValidTo.Value < now))
                                {
                                    context.Fail(
                                        "Kullanıcı geçerlilik aralığı uygun değil.");
                                }
                            }
                    };
            });

        services.AddAuthorization(
            options =>
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
