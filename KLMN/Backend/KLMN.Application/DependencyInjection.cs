using FluentValidation;
using KLMN.Application.Common.Authorization;
using KLMN.Application.Common.Behaviors;
using KLMN.Application.Common.Interfaces.Authorization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace KLMN.Application;

/// <summary>Application katmanının DI kayıtlarını içerir.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// MediatR, FluentValidation ve yetkilendirme hizmetlerini DI container'a kaydeder.
    /// </summary>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Application katmanındaki MediatR handler ve FluentValidation validatorlarının bulunduğu derlemedir.
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();

        return services;
    }
}
