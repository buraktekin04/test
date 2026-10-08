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
    /// <summary>
    /// Npgsql DbContext, audit interceptor ve başlangıç seed servislerini DI sistemine ekler.
    /// </summary>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Ortama göre appsettings.json veya appsettings.Development.json üzerinden alınan PostgreSQL bağlantısıdır.
        var connectionString =
            configuration.GetConnectionString("PostgreSQL");

        /*
         * Veritabanı sunucusu, port, veritabanı adı, kullanıcı ve
         * parola seçili ortamın appsettings.json veya
         * appsettings.Development.json içindeki
         * ConnectionStrings:PostgreSQL alanında tanımlanır.
         */
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Seçili ortamın appsettings dosyasındaki ConnectionStrings:PostgreSQL boş bırakılamaz.");
        }

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
