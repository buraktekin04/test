using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Hospitals.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Hospitals.Commands.Common;

/// <summary>Sağlık Tesisi kod tekilliği ve xmin concurrency iş kuralları.</summary>
internal static class HospitalDefRules
{
    /// <summary>Silinmemiş kayıtlar arasında büyük/küçük harf duyarsız kod kontrolü.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? existingId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.HospitalDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!existingId.HasValue || x.Id != existingId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new HospitalConflictException("Bu sağlık tesisi kodu zaten kullanılıyor.");
    }

    /// <summary>Güncel xmin'i doğrular; eski kayıt sürümüyle değişikliği engeller.</summary>
    public static async Task<HospitalDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.HospitalDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new HospitalNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Sağlık tesisi başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
    /// <summary>Yeni seçilen şehir ve hakem tesis aktif olmalıdır; eski pasif seçim korunabilir.</summary>
    public static async Task EnsureRelationsAsync(IKLMNDbContext db, Guid? cityId, Guid? refereeId,
        Guid? selfId, CancellationToken ct, Guid? previousCity = null, Guid? previousReferee = null)
    {
        if (refereeId.HasValue && selfId == refereeId)
            throw new HospitalConflictException("Sağlık tesisi kendisinin hakem tesisi olamaz.");
        if (cityId.HasValue && cityId != previousCity &&
            !await db.CityDefs.AnyAsync(x => x.Id == cityId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new HospitalConflictException("Şehir aktif ve silinmemiş olmalıdır.");
        if (refereeId.HasValue && refereeId != previousReferee &&
            !await db.HospitalDefs.AnyAsync(x => x.Id == refereeId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new HospitalConflictException("Hakem sağlık tesisi aktif ve silinmemiş olmalıdır.");
        // A -> B -> C -> A gibi döngüleri engelle.
        if (refereeId.HasValue && selfId.HasValue && refereeId != previousReferee)
        {
            var cursor = refereeId;
            var visited = new HashSet<Guid>();
            while (cursor.HasValue)
            {
                if (cursor == selfId || !visited.Add(cursor.Value))
                    throw new HospitalConflictException("Hakem sağlık tesisi ilişkisi döngü oluşturamaz.");
                cursor = await db.HospitalDefs.IgnoreQueryFilters().AsNoTracking()
                    .Where(x => x.Id == cursor.Value && !x.IsDeleted)
                    .Select(x => x.RefereeHospitalId).SingleOrDefaultAsync(ct);
            }
        }
    }

    /// <summary>Bağlı tesis veya rapor türü bulunan tesis silinemez/pasifleştirilemez.</summary>
    public static async Task EnsureNoDependentsAsync(IKLMNDbContext db, Guid id, bool deactivate, CancellationToken ct)
    {
        if (await db.HospitalDefs.IgnoreQueryFilters().AnyAsync(x => !x.IsDeleted &&
            x.RefereeHospitalId == id && (!deactivate || x.IsActive), ct))
            throw new HospitalConflictException("Bu sağlık tesisine bağlı hakem sağlık tesisi ilişkisi bulunuyor.");
        if (await db.HospitalReportTypes.IgnoreQueryFilters().AnyAsync(x => !x.IsDeleted &&
            x.HospitalDefId == id && (!deactivate || x.IsActive), ct))
            throw new HospitalConflictException("Sağlık tesisi ile rapor türü eşleştirmeleri bulunuyor.");
    }
}

