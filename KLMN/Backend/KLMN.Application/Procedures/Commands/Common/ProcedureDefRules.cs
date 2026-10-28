using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Procedures.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Procedures.Commands.Common;

/// <summary>Tetkik kod tekilliği ve xmin concurrency iş kuralları.</summary>
internal static class ProcedureDefRules
{
    /// <summary>Silinmemiş kayıtlar arasında büyük/küçük harf duyarsız kod kontrolü.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? existingId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.ProcedureDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!existingId.HasValue || x.Id != existingId.Value) &&
            x.ProcedureCode.ToUpper() == normalized, ct))
            throw new ProcedureConflictException("Bu tetkik kodu zaten kullanılıyor.");
    }

    /// <summary>Yeni seçimde mevcut ve aktif tetkik türünü doğrular.</summary>
    /// <summary>Kodun aktif kaydını bulur; ilişki değişmez GUID ile kurulmalıdır.</summary>
    public static async Task<Guid> ResolveTypeIdAsync(IKLMNDbContext db,int code,CancellationToken ct)
    {
        var id=await db.ProcedureTypeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x=>x.Code==code&&!x.IsDeleted&&x.IsActive)
            .Select(x=>x.Id).FirstOrDefaultAsync(ct);
        if(id==Guid.Empty)
            throw new ProcedureConflictException("Aktif bir Tetkik Türü seçilmelidir.");
        return id;
    }

    /// <summary>Güncel xmin'i doğrular; eski kayıt sürümüyle değişikliği engeller.</summary>
    public static async Task<ProcedureDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.ProcedureDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new ProcedureNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
}
