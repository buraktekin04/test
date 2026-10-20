using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Ranks.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Ranks.Commands.Common;

/// <summary>Rütbe kod tekilliği ve xmin concurrency iş kuralları.</summary>
internal static class RankDefRules
{
    /// <summary>Silinmemiş kayıtlar arasında büyük/küçük harf duyarsız kod kontrolü.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? existingId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.RankDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!existingId.HasValue || x.Id != existingId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new RankConflictException("Bu rütbe kodu zaten kullanılıyor.");
    }

    /// <summary>Güncel xmin'i doğrular; eski kayıt sürümüyle değişikliği engeller.</summary>
    public static async Task<RankDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.RankDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new RankNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Karar kaydı başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
    /// <summary>Yeni seçimde grubu aktif olmalı, mevcut pasif seçim ise düzenlemede korunabilir.</summary>
    public static async Task EnsureGroupAsync(
        IKLMNDbContext db, Guid? groupId, CancellationToken ct, Guid? unchangedGroupId = null)
    {
        if (!groupId.HasValue || groupId == unchangedGroupId) return;
        if (!await db.RankGroupDefs.AnyAsync(x => x.Id == groupId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new RankConflictException("Seçilen rütbe grubu aktif ve silinmemiş olmalıdır.");
    }
}

