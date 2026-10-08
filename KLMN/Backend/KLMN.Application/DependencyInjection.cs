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
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
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
