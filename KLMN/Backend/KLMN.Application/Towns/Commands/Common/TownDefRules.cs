using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Towns.Exceptions;
using KLMN.Domain.Entities.Geography;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Towns.Commands.Common;

/// <summary>
/// İlçe kod benzersizliği, optimistic concurrency ve aktif şehir kontrolünü
/// tüm oluşturma/güncelleme/durum komutlarında ortak uygular.
/// </summary>
internal static class TownDefRules
{
    /// <summary>Silinmemiş kayıtlarda büyük/küçük harf duyarsız kod eşsizliğini sağlar.</summary>
    public static async Task EnsureUniqueCodeAsync(IKLMNDbContext db, string code, Guid? excludedId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.TownDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!excludedId.HasValue || x.Id != excludedId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new TownConflictException("Bu ilçe kodu zaten kayıtlıdır.");
    }

    /// <summary>Silinmemiş kaydı getirir; istemci xmin eskidiyse 409 çakışma üretir.</summary>
    public static async Task<TownDef> LoadForChangeAsync(IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var entity = await db.TownDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new TownNotFoundException();
        if (entity.Version != version)
            throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirilmiş; listeyi yenileyiniz.");
        return entity;
    }

    /// <summary>
    /// Yeni bağlantı ve aktifleştirmede şehrin aktif olmasını zorunlu tutar;
    /// daha önce bağlı pasif şehri değiştirmeden düzenlemeye izin verir.
    /// </summary>
    public static async Task EnsureCityAsync(
        IKLMNDbContext db, Guid? cityId, CancellationToken ct, Guid? unchangedCityId = null)
    {
        // Pasifleştirilmiş bir şehirye bağlı geçmiş ilçe kaydı mevcut bağlantısını koruyabilir.
        // Yeni bağlama veya ilçe aktivasyonu bu istisnadan yararlanamaz.
        if (!cityId.HasValue || (unchangedCityId.HasValue && cityId == unchangedCityId))
            return;
        if (!await db.CityDefs.AnyAsync(x => x.Id == cityId.Value && x.IsActive && !x.IsDeleted, ct))
            throw new TownConflictException("Seçilen şehir aktif ve geçerli olmalıdır.");
    }

}
