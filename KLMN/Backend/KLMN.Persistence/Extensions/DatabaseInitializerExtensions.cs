using KLMN.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KLMN.Persistence.Extensions;

/// <summary>
/// Uygulama başlangıcında migration ve seed işlemlerini çalıştırır.
/// </summary>
public static class DatabaseInitializerExtensions
{
    /// <summary>
    /// Migration'ları uygular ve temel identity verilerini seed eder.
/// </summary>
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope =
            services.CreateAsyncScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<KLMNDbContext>();

        await context.Database.MigrateAsync(
            cancellationToken);

        var permissionSeeder =
            scope.ServiceProvider
                .GetRequiredService<PermissionSeeder>();

        await permissionSeeder.SeedAsync(
            cancellationToken);

        var identitySeeder =
            scope.ServiceProvider
                .GetRequiredService<IdentitySeeder>();

        await identitySeeder.SeedAsync(
            cancellationToken);
    }
}
