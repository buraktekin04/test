using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.RankGroups.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.RankGroups.Commands.Common;

/// <summary>Rütbe Grubu kod tekilliği ve xmin concurrency iş kuralları.</summary>
internal static class RankGroupDefRules
{
    /// <summary>Silinmemiş kayıtlar arasında büyük/küçük harf duyarsız kod kontrolü.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? existingId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.RankGroupDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!existingId.HasValue || x.Id != existingId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new RankGroupConflictException("Bu rütbe grubu kodu zaten kullanılıyor.");
    }

    /// <summary>Güncel xmin'i doğrular; eski kayıt sürümüyle değişikliği engeller.</summary>
    public static async Task<RankGroupDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.RankGroupDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new RankGroupNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
    /// <summary>İlişkili kayıtlar varken grubu silmeyi veya aktif rütbeler varken pasifleştirmeyi engeller.</summary>
    public static async Task EnsureNoRanksAsync(IKLMNDbContext db, Guid groupId, bool deactivate, CancellationToken ct)
    {
        var linked = await db.RankDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && x.RankGroupId == groupId && (!deactivate || x.IsActive), ct);
        if (linked)
            throw new RankGroupConflictException(deactivate
                ? "Aktif rütbeler bağlıyken rütbe grubu pasifleştirilemez."
                : "Bağlı rütbeler bulunan rütbe grubu silinemez.");
    }
}

