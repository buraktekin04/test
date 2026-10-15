using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitarySchools.Exceptions;
using KLMN.Domain.Entities.MilitaryUnit;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitarySchools.Commands.Common;

/// <summary>Okul kodu, iki nullable FK ve güncel xmin iş kurallarını merkezileştirir.</summary>
internal static class MilitarySchoolDefRules
{
    /// <summary>Silinmemiş okullar arasında kodun case-insensitive tekilliğini denetler.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? currentId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.MilitarySchoolDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!currentId.HasValue || x.Id != currentId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new MilitarySchoolConflictException("Bu askerî okul kodu zaten kullanılıyor.");
    }

    /// <summary>Kaydı ve optimistic concurrency sürümünü denetler; değişmiş kayıt için 409.</summary>
    public static async Task<MilitarySchoolDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var school = await db.MilitarySchoolDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new MilitarySchoolNotFoundException();
        if (school.Version != version)
            throw new DbUpdateConcurrencyException("Okul başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return school;
    }

    /// <summary>
    /// Yeni FK seçimi veya okulun etkinleştirilmesi aktif tanımları gerektirir.
    /// Düzenlemede değişmeyen mevcut pasif bağlantılar geçmişi bozmaz.
    /// </summary>
    public static async Task EnsureReferencesAsync(
        IKLMNDbContext db, Guid? cityId, Guid? forceId,
        CancellationToken ct, Guid? unchangedCityId = null, Guid? unchangedForceId = null)
    {
        if (cityId.HasValue && cityId != unchangedCityId &&
            !await db.CityDefs.AnyAsync(x => x.Id == cityId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new MilitarySchoolConflictException("Seçilen şehir aktif ve silinmemiş olmalıdır.");
        if (forceId.HasValue && forceId != unchangedForceId &&
            !await db.ForcesCommandDefs.AnyAsync(x => x.Id == forceId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new MilitarySchoolConflictException("Seçilen kuvvet komutanlığı aktif ve silinmemiş olmalıdır.");
    }
}
