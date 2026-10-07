using FluentValidation;
using KLMN.Application.Common.Behaviors;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Services;
using KLMN.Application.Common.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KLMN.Application;

/// <summary>
/// Application katmanının dependency injection kayıtlarını içerir.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// KLMN Application servislerini DI container'a ekler.
    /// </summary>
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly =
            typeof(DependencyInjection).Assembly;

        services.AddMediatR(
            options =>
                options.RegisterServicesFromAssembly(
                    assembly));

        services.AddValidatorsFromAssembly(
            assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddScoped<
            IUserAuthorizationService,
            UserAuthorizationService>();

        services.AddScoped<
            IPermissionChecker,
            PermissionChecker>();

        services.Configure<AuthenticationSettings>(
            configuration.GetSection(
                AuthenticationSettings.SectionName));

        services.Configure<PasswordResetSettings>(
            configuration.GetSection(
                PasswordResetSettings.SectionName));

        return services;
    }
}
