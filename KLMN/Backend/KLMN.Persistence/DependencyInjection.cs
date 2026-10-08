using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Persistence.Contexts;
using KLMN.Persistence.Interceptors;
using KLMN.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KLMN.Persistence;

/// <summary>Persistence katmanının DI kayıtlarını içerir.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException(
                "PostgreSQL connection string bulunamadı.");

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<KLMNDbContext>(
            (serviceProvider, options) =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(
                            typeof(KLMNDbContext).Assembly.FullName);

                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);
                    });

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<
                        AuditableEntitySaveChangesInterceptor>());
            });

        services.AddScoped<IKLMNDbContext>(
            sp => sp.GetRequiredService<KLMNDbContext>());

        services.AddScoped<PermissionSeeder>();
        services.AddScoped<IdentitySeeder>();

        return services;
    }
}
