using KLMN.Persistence.Contexts;
using KLMN.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KLMN.Persistence.Extensions;

/// <summary>Migration ve başlangıç seed işlemlerini çalıştırır.</summary>
public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<KLMNDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        var identitySeeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
        await identitySeeder.SeedAsync(cancellationToken);
    }
}
