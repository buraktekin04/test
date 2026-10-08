using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence.Seeds;

/// <summary>Permission seed kayıtlarını idempotent olarak senkronize eder.</summary>
internal sealed class PermissionSeeder
{
    private readonly KLMNDbContext _dbContext;

    public PermissionSeeder(KLMNDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
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
