using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryOffices.Exceptions;
using KLMN.Domain.Entities.MilitaryUnit;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryOffices.Commands.Common;

/// <summary>Tekil kod, şehir bağı ve xmin iş kuralları.</summary>
internal static class MilitaryOfficeDefRules
{
    /// <summary>Silinmemiş şubelerde kod tekilliğini doğrular.</summary>
    public static async Task EnsureUniqueCodeAsync(IKLMNDbContext db, string code, Guid? id, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.MilitaryOfficeDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!id.HasValue || x.Id != id.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new MilitaryOfficeConflictException("Bu askerlik şubesi kodu zaten kullanılıyor.");
    }

    /// <summary>Silinmemiş kaydı ve eşzamanlılık sürümünü denetler.</summary>
    public static async Task<MilitaryOfficeDef> LoadForChangeAsync(IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.MilitaryOfficeDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new MilitaryOfficeNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }

    /// <summary>Yeni bağlantıda aktif şehir gerekir; geçmiş değişmeyen FK düzenlemede kalabilir.</summary>
    public static async Task EnsureCityAsync(
        IKLMNDbContext db, Guid? cityId, CancellationToken ct, Guid? unchangedCityId = null)
    {
        if (!cityId.HasValue || cityId.Value == Guid.Empty)
            throw new MilitaryOfficeConflictException("Şehir seçimi zorunludur.");
        if (cityId == unchangedCityId) return;
        if (!await db.CityDefs.AnyAsync(x => x.Id == cityId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new MilitaryOfficeConflictException("Seçilen şehir aktif ve silinmemiş olmalıdır.");
    }
}
