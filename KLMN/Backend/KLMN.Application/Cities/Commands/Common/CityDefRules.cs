using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Cities.Exceptions;
using KLMN.Domain.Entities.Geography;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Cities.Commands.Common;

internal static class CityDefRules
{
    public static async Task EnsureUniqueCodeAsync(IKLMNDbContext db, string code, Guid? excludedId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.CityDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!excludedId.HasValue || x.Id != excludedId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new CityConflictException("Bu şehir kodu zaten kayıtlıdır.");
    }

    public static async Task<CityDef> LoadForChangeAsync(IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var entity = await db.CityDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new CityNotFoundException();
        if (entity.Version != version)
            throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirilmiş; listeyi yenileyiniz.");
        return entity;
    }

    public static async Task EnsureCountryAsync(
        IKLMNDbContext db, Guid? countryId, CancellationToken ct, Guid? unchangedCountryId = null)
    {
        // Pasifleştirilmiş bir ülkeye bağlı geçmiş şehir kaydı mevcut bağlantısını koruyabilir.
        // Yeni bağlama veya şehir aktivasyonu bu istisnadan yararlanamaz.
        if (!countryId.HasValue || (unchangedCountryId.HasValue && countryId == unchangedCountryId))
            return;
        if (!await db.CountryDefs.AnyAsync(x => x.Id == countryId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new CityConflictException("Seçilen ülke aktif ve geçerli olmalıdır.");
    }

}
