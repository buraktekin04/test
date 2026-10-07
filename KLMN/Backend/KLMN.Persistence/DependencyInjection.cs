using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Persistence.Interceptors;
using KLMN.Persistence.Seeds;
using KLMN.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KLMN.Persistence;

/// <summary>
/// Persistence katmanı dependency injection kayıtlarını içerir.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// PostgreSQL DbContext, interceptor ve seed servislerini kaydeder.
/// </summary>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<InitialAdminSettings>(
            configuration.GetSection(
                InitialAdminSettings.SectionName));

        services.AddScoped<
            AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<KLMNDbContext>(
            (serviceProvider, options) =>
            {
                var connectionString =
                    configuration.GetConnectionString(
                        "DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "DefaultConnection bulunamadı.");

                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsHistoryTable(
                            "__EFMigrationsHistory",
                            "klmn");
                    });

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<
                        AuditableEntitySaveChangesInterceptor>());
            });

        services.AddScoped<IKLMNDbContext>(
            serviceProvider =>
                serviceProvider.GetRequiredService<
                    KLMNDbContext>());

        services.AddScoped<PermissionSeeder>();
        services.AddScoped<IdentitySeeder>();

        return services;
    }
}
