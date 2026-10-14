using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PurchaseNumbers.Exceptions;
using KLMN.Domain.Entities.PurchaseNumbers;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.PurchaseNumbers.Commands.Common;

internal static class PurchaseNumberDefRules
{
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? exceptId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.PurchaseNumberDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!exceptId.HasValue || x.Id != exceptId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new PurchaseNumberConflictException("Bu alım numarası kodu zaten kullanılıyor.");
    }

    public static async Task<PurchaseNumberDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var entity = await db.PurchaseNumberDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new PurchaseNumberNotFoundException();
        if (entity.Version != version)
            throw new DbUpdateConcurrencyException(
                "Alım numarası başka bir kullanıcı tarafından değiştirilmiş. Kaydı yenileyiniz.");
        return entity;
    }
}
