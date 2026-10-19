using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Branches.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Branches.Commands.Common;

/// <summary>Branş kod tekilliği ve xmin concurrency iş kuralları.</summary>
internal static class BranchDefRules
{
    /// <summary>Silinmemiş kayıtlar arasında büyük/küçük harf duyarsız kod kontrolü.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? existingId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.BranchDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!existingId.HasValue || x.Id != existingId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new BranchConflictException("Bu branş kodu zaten kullanılıyor.");
    }

    /// <summary>Güncel xmin'i doğrular; eski kayıt sürümüyle değişikliği engeller.</summary>
    public static async Task<BranchDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.BranchDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new BranchNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Karar kaydı başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
}
