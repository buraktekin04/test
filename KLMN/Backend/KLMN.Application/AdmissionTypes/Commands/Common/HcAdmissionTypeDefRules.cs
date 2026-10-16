using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.AdmissionTypes.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.AdmissionTypes.Commands.Common;

/// <summary>Başvuru grubu kod tekilliği ve optimistic concurrency iş kuralları.</summary>
internal static class HcAdmissionTypeDefRules
{
    /// <summary>Soft-delete olmayan gruplarda sayısal kod tekilliğini korur.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, int code, Guid? excludedId, CancellationToken ct)
    {
        if (await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AnyAsync(
            x => !x.IsDeleted && x.Code == code &&
                 (!excludedId.HasValue || x.Id != excludedId.Value), ct))
            throw new AdmissionTypeConflictException("Bu başvuru grubu kodu zaten kullanılıyor.");
    }

    /// <summary>Silinmemiş kaydı getirir; eski xmin sürümünde 409 üretilmesini sağlar.</summary>
    public static async Task<HcAdmissionTypeDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.HcAdmissionTypeDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new AdmissionTypeNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Başvuru grubu başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
}
