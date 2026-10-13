using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Exceptions;
using KLMN.Domain.Entities.MilitaryUnit;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Commands.Common;

/// <summary>Birlik hiyerarşisi, benzersizlik ve concurrency ortak domain kurallarıdır.</summary>
internal static class MilitaryUnitDefRules
{
    /// <summary>Yeni atamalarda kuvvetin varlığını, aktifliğini ve tarih aralığını doğrular.</summary>
    public static async Task EnsureValidForceAsync(IKLMNDbContext db, Guid? forceId, CancellationToken ct)
    {
        if (!forceId.HasValue) return;
        var now = DateTime.UtcNow;
        if (!await db.ForcesCommandDefs.IgnoreQueryFilters().AnyAsync(x => x.Id == forceId &&
            !x.IsDeleted && x.IsActive && (!x.ValidFrom.HasValue || x.ValidFrom <= now) &&
            (!x.ValidTo.HasValue || now < x.ValidTo), ct))
            throw new MilitaryUnitConflictException("Seçilen kuvvet komutanlığı bulunamadı, pasif veya geçerlilik dönemi dışında.");
    }

    /// <summary>Pasifler dahil aynı teknik koda sahip, silinmemiş birim bulunmamalıdır.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? currentId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.MilitaryUnitDefs.IgnoreQueryFilters()
                .AnyAsync(x => !x.IsDeleted &&
                    (!currentId.HasValue || x.Id != currentId.Value) &&
                    x.Code.ToUpper() == normalized, ct))
            throw new MilitaryUnitConflictException("Bu birlik kodu zaten kullanılıyor.");
    }

    /// <summary>Güncel xmin değerini kontrol ederek mevcut birimi yükler.</summary>
    public static async Task<MilitaryUnitDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var unit = await db.MilitaryUnitDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new MilitaryUnitNotFoundException();

        if (unit.Version != version)
            throw new DbUpdateConcurrencyException(
                "Birlik kaydı başka bir işlemde değiştirildi. Listeyi yenileyiniz.");

        return unit;
    }

    /// <summary>
    /// Seçilen üst birimin aktif olduğunu ve hiyerarşide döngü oluşmadığını denetler.
    /// Root (null) seçimi serbesttir. Döngü kontrolü sunucu tarafındadır.
    /// </summary>
    public static async Task EnsureValidParentAsync(
        IKLMNDbContext db, Guid? parentId, Guid? currentId, CancellationToken ct)
    {
        if (parentId is null) return;

        var visited = new HashSet<Guid>();
        var next = parentId;
        while (next.HasValue)
        {
            if (next.Value == currentId || !visited.Add(next.Value))
                throw new MilitaryUnitConflictException(
                    "Bir birlik kendisine veya kendi alt birimlerine bağlanamaz.");

            var parent = await db.MilitaryUnitDefs.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.Id == next.Value && !x.IsDeleted && x.IsActive)
                .Select(x => new { x.ConnectedTopUnity })
                .SingleOrDefaultAsync(ct);

            if (parent is null)
                throw new MilitaryUnitConflictException(
                    "Seçilen üst birim bulunamadı veya pasif. Aktif bir üst birim seçiniz.");

            next = parent.ConnectedTopUnity;
        }
    }
}
