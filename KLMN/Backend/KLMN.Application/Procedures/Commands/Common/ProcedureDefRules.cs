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
            x.Code.ToUpper() == normalized, ct))
            throw new ProcedureConflictException("Bu tetkik kodu zaten kullanılıyor.");
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
