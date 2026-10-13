using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Organizations.Exceptions;
using KLMN.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Organizations.Commands.Common;

/// <summary>Organizasyon hiyerarşisi, benzersizlik ve concurrency ortak domain kurallarıdır.</summary>
internal static class OrganizationUnitRules
{
    /// <summary>Pasifler dahil aynı teknik koda sahip, silinmemiş birim bulunmamalıdır.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? currentId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.OrganizationUnits.IgnoreQueryFilters()
                .AnyAsync(x => !x.IsDeleted &&
                    (!currentId.HasValue || x.Id != currentId.Value) &&
                    x.Code.ToUpper() == normalized, ct))
            throw new OrganizationConflictException("Bu organizasyon kodu zaten kullanılıyor.");
    }

    /// <summary>Güncel xmin değerini kontrol ederek mevcut birimi yükler.</summary>
    public static async Task<OrganizationUnit> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var unit = await db.OrganizationUnits.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new OrganizationNotFoundException();

        if (unit.Version != version)
            throw new DbUpdateConcurrencyException(
                "Organizasyon kaydı başka bir işlemde değiştirildi. Listeyi yenileyiniz.");

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
                throw new OrganizationConflictException(
                    "Bir organizasyon birimi kendisine veya kendi alt birimlerine bağlanamaz.");

            var parent = await db.OrganizationUnits.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.Id == next.Value && !x.IsDeleted && x.IsActive)
                .Select(x => new { x.ParentOrganizationUnitId })
                .SingleOrDefaultAsync(ct);

            if (parent is null)
                throw new OrganizationConflictException(
                    "Seçilen üst birim bulunamadı veya pasif. Aktif bir üst birim seçiniz.");

            next = parent.ParentOrganizationUnitId;
        }
    }
}
