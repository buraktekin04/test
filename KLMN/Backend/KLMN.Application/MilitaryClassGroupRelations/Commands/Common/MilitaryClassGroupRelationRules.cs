using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroupRelations.Exceptions;
using KLMN.Domain.Entities.MilitaryUnit;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.Common;

/// <summary>
/// Üçlü ilişkinin referanslarını, kuvvet-grup tutarlılığını,
/// mükerrer kaydı ve optimistic concurrency sürümünü korur.
/// </summary>
public static class MilitaryClassGroupRelationRules
{
    /// <summary>Seçilen grup seçilen kuvvete ait olmalı; her üç kayıt aktif olmalıdır.</summary>
    public static async Task EnsureReferencesAsync(IKLMNDbContext db, Guid force, Guid militaryClass, Guid group, CancellationToken ct)
    {
        if (!await db.ForcesCommandDefs.AnyAsync(x => x.Id == force && x.IsActive && !x.IsDeleted, ct))
            throw new MilitaryClassGroupRelationConflictException("Aktif kuvvet komutanlığı seçiniz.");
        if (!await db.MilitaryClassDefs.AnyAsync(x => x.Id == militaryClass && x.IsActive && !x.IsDeleted, ct))
            throw new MilitaryClassGroupRelationConflictException("Aktif askerî sınıf seçiniz.");
        if (!await db.MilitaryClassGroupDefs.AnyAsync(x => x.Id == group && x.ForcesCommandId == force && x.IsActive && !x.IsDeleted, ct))
            throw new MilitaryClassGroupRelationConflictException("Seçilen grup bu kuvvete ait ve aktif olmalıdır.");
    }

    /// <summary>Aynı kuvvet-sınıf-grup eşleştirmesini birden fazla kez oluşturmaz.</summary>
    public static async Task EnsureUniqueAsync(IKLMNDbContext db, Guid force, Guid militaryClass, Guid group, Guid? id, CancellationToken ct)
    {
        if (await db.MilitaryClassGroupRelations.IgnoreQueryFilters().AnyAsync(x => !x.IsDeleted
            && x.ForcesCommandId == force && x.MilitaryClassId == militaryClass
            && x.MilitaryClassGroupId == group && (!id.HasValue || x.Id != id.Value), ct))
            throw new MilitaryClassGroupRelationConflictException("Bu sınıf, grup ve kuvvet eşleştirmesi zaten var.");
    }

    /// <summary>Yalnızca güncel xmin ile değiştirilebilir soft-delete olmayan kaydı getirir.</summary>
    public static async Task<MilitaryClassGroupRelation> LoadAsync(IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.MilitaryClassGroupRelations.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new MilitaryClassGroupRelationNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Eşleştirme başka kullanıcı tarafından değiştirildi; yenileyiniz.");
        return item;
    }
}
