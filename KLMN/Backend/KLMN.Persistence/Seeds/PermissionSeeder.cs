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

        // Eski organizasyon izinlerinin kimliğini koru: rol/kullanıcı atamaları kaybolmaz.
        foreach (var permission in existingPermissions.Values.ToList())
        {
            if (!permission.Code.StartsWith("Organizations.", StringComparison.Ordinal)) continue;
            var newCode = "MilitaryUnits." + permission.Code["Organizations.".Length..];
            if (existingPermissions.ContainsKey(newCode))
                throw new InvalidOperationException("Eski ve yeni birlik izinleri birlikte mevcut; izin atamaları birleştirilmeden devam edilemez.");
            existingPermissions.Remove(permission.Code);
            permission.Code = newCode;
            permission.Module = "MilitaryUnits";
            existingPermissions.Add(newCode, permission);
        }

        // Kaldırılan modülün veritabanında kalmış permission satırları denetim geçmişi
        // için korunur, fakat yeni oturumlarda erişime kapalı tutulur.
        foreach (var stale in existingPermissions.Values.Where(x =>
                     x.Code.StartsWith("PurchaseNumbers.", StringComparison.OrdinalIgnoreCase)))
        {
            stale.IsActive = false;
        }

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
