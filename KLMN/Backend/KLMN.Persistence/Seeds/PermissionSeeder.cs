using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence.Seeds;

/// <summary>Permission seed kayıtlarını idempotent olarak senkronize eder.</summary>
internal sealed class PermissionSeeder
{
    /// <summary>
    /// EF Core üzerinden identity, rol ve permission kayıtlarına erişen PostgreSQL DbContext'tir.
    /// </summary>
    private readonly KLMNDbContext _dbContext;

    /// <summary>
    /// permission seeder işlemini ilgili persistence sorumluluğuyla yerine getirir.
    /// </summary>
    public PermissionSeeder(KLMNDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Merkezi rollerin ve izinlerin idempotent veritabanı başlangıç kayıtlarını oluşturur.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Kod eşleştirmesiyle tekrar seed edilmesi önlenen tüm mevcut izinlerin sözlüğüdür.
        var existingPermissions = await _dbContext.Permissions
            .IgnoreQueryFilters()
            .ToDictionaryAsync(
                x => x.Code,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        foreach (var seedItem in IdentitySeedData.Permissions)
        {
            if (existingPermissions.TryGetValue(seedItem.Code, out var permission))
            {
                permission.Name = seedItem.Name;
                permission.Module = seedItem.Module;
                permission.SortOrder = seedItem.SortOrder;
                permission.IsDeleted = false;
                permission.IsActive = true;
                permission.DeletedDate = null;
                permission.DeletedBy = null;
                continue;
            }

            await _dbContext.Permissions.AddAsync(
                new Permission
                {
                    Name = seedItem.Name,
                    Code = seedItem.Code,
                    Module = seedItem.Module,
                    SortOrder = seedItem.SortOrder
                },
                cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
