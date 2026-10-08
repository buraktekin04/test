using KLMN.Persistence.Contexts;
using KLMN.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KLMN.Persistence.Extensions;

/// <summary>Migration ve başlangıç seed işlemlerini çalıştırır.</summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// EF Core migration'larını uygular ve identity seed işlemini başlatır.
    /// </summary>
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        // Migration ve seed işlemlerinde kullanılacak scoped PostgreSQL context örneğidir.
        var dbContext = scope.ServiceProvider.GetRequiredService<KLMNDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        // İlk rol, yetki ve yönetici kullanıcısını hazırlayan seed servisidir.
        var identitySeeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
        await identitySeeder.SeedAsync(cancellationToken);
    }
}
