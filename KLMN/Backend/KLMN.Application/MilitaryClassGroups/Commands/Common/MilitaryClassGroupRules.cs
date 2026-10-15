using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroups.Exceptions;
using KLMN.Domain.Entities.MilitaryUnit;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroups.Commands.Common;

/// <summary>Grup kodu benzersizliği, kuvvet bağı, xmin ve ilişkili eşleştirme korumaları.</summary>
public static class MilitaryClassGroupRules
{
    /// <summary>Kuvvet içinde kodun (silinmemiş kayıtlar arasında) tekil olmasını sağlar.</summary>
    public static async Task EnsureUniqueAsync(IKLMNDbContext db, Guid forceId, string code, Guid? id, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.MilitaryClassGroupDefs.IgnoreQueryFilters().AnyAsync(x => !x.IsDeleted
            && x.ForcesCommandId == forceId && (!id.HasValue || x.Id != id.Value)
            && x.Code.ToUpper() == normalized, ct))
            throw new MilitaryClassGroupConflictException("Bu kuvvet için grup kodu zaten tanımlı.");
    }

    /// <summary>Yeni kuvvet bağlantıları yalnızca aktif/silinmemiş kuvvetlere yapılabilir.</summary>
    public static async Task EnsureForceAsync(IKLMNDbContext db, Guid forceId, CancellationToken ct)
    {
        if (!await db.ForcesCommandDefs.AnyAsync(x => x.Id == forceId && x.IsActive && !x.IsDeleted, ct))
            throw new MilitaryClassGroupConflictException("Aktif bir kuvvet komutanlığı seçiniz.");
    }

    /// <summary>Version değeri eskidiyse 409 oluşturan optimistic concurrency kontrolü.</summary>
    public static async Task<MilitaryClassGroupDef> LoadAsync(IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.MilitaryClassGroupDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new MilitaryClassGroupNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Grup başka kullanıcı tarafından değiştirildi; yenileyiniz.");
        return item;
    }

    /// <summary>Silmede bütün eşleştirmeleri; pasifleştirmede aktif eşleştirmeleri korur.</summary>
    public static async Task EnsureNoRelationsAsync(IKLMNDbContext db, Guid groupId, bool deactivate, CancellationToken ct)
    {
        var exists = await db.MilitaryClassGroupRelations.IgnoreQueryFilters().AnyAsync(x =>
            x.MilitaryClassGroupId == groupId && !x.IsDeleted && (!deactivate || x.IsActive), ct);
        if (exists) throw new MilitaryClassGroupConflictException(deactivate
            ? "Aktif eşleştirmesi bulunan grup pasifleştirilemez."
            : "Eşleştirmesi bulunan grup silinemez veya başka kuvvete taşınamaz.");
    }
}
