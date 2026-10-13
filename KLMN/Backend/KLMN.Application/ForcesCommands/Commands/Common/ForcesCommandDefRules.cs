using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ForcesCommands.Exceptions;
using KLMN.Domain.Entities.MilitaryUnit;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ForcesCommands.Commands.Common;

internal static class ForcesCommandDefRules
{
    public static async Task EnsureUniqueCodeAsync(IKLMNDbContext db, string code, Guid? currentId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.ForcesCommandDefs.IgnoreQueryFilters().AnyAsync(x => !x.IsDeleted &&
            (!currentId.HasValue || x.Id != currentId.Value) && x.Code.ToUpper() == normalized, ct))
            throw new ForcesCommandConflictException("Bu kuvvet komutanlığı kodu zaten kullanılıyor.");
    }

    public static async Task<ForcesCommandDef> LoadForChangeAsync(IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var force = await db.ForcesCommandDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new ForcesCommandNotFoundException();
        if (force.Version != version)
            throw new DbUpdateConcurrencyException("Kuvvet komutanlığı başka bir işlemde değiştirildi. Listeyi yenileyiniz.");
        return force;
    }
}
