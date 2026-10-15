using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Exceptions;
using KLMN.Domain.Entities.MilitaryUnit;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClasses.Commands.Common;

/// <summary>Askerî sınıf kod tekilliği ve optimistic concurrency kontrolünü merkezileştirir.</summary>
internal static class MilitaryClassDefRules
{
    /// <summary>Soft-delete olmayan sınıflarda kodun büyük/küçük harf duyarsız tekilliğini denetler.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? currentId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.MilitaryClassDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!currentId.HasValue || x.Id != currentId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new MilitaryClassConflictException("Bu askerî sınıf kodu zaten kullanılıyor.");
    }

    /// <summary>Silinmemiş kaydı bulur ve istemcinin xmin değerinin güncel olduğunu doğrular.</summary>
    public static async Task<MilitaryClassDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.MilitaryClassDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new MilitaryClassNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Sınıf başka kullanıcı tarafından değiştirildi. Listeyi yenileyiniz.");
        return item;
    }
}
