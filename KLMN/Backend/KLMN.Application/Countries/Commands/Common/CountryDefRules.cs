using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Countries.Exceptions;
using KLMN.Domain.Entities.Geography;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Countries.Commands.Common;

internal static class CountryDefRules
{
    public static async Task EnsureUniqueCodeAsync(IKLMNDbContext db, string code, Guid? excludedId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.CountryDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!excludedId.HasValue || x.Id != excludedId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new CountryConflictException("Bu ülke kodu zaten kayıtlıdır.");
    }

    public static async Task<CountryDef> LoadForChangeAsync(IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var entity = await db.CountryDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new CountryNotFoundException();
        if (entity.Version != version)
            throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirilmiş; listeyi yenileyiniz.");
        return entity;
    }

    public static async Task EnsureNoCitiesAsync(IKLMNDbContext db, Guid id, bool deactivate, CancellationToken ct)
    {
        var linked = await db.CityDefs.IgnoreQueryFilters().AnyAsync(x =>
            x.CountryId == id && !x.IsDeleted && (!deactivate || x.IsActive), ct);
        if (linked)
            throw new CountryConflictException(
                deactivate ? "Aktif şehri bulunan ülke pasifleştirilemez." :
                             "Bağlı şehri bulunan ülke silinemez.");
    }
}
