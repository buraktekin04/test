using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ConnectedDecisions.Commands.Common;

/// <summary>Bağlı kurul kararı kod tekilliği ve xmin concurrency iş kuralları.</summary>
internal static class HcConnectedDecisionDefRules
{
    /// <summary>Silinmemiş kayıtlar arasında büyük/küçük harf duyarsız kod kontrolü.</summary>
    public static async Task EnsureUniqueCodeAsync(
        IKLMNDbContext db, string code, Guid? existingId, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        if (await db.HcConnectedDecisionDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && (!existingId.HasValue || x.Id != existingId.Value) &&
            x.Code.ToUpper() == normalized, ct))
            throw new ConnectedDecisionConflictException("Bu bağlı kurul kararı kodu zaten kullanılıyor.");
    }

    /// <summary>Güncel xmin'i doğrular; eski kayıt sürümüyle değişikliği engeller.</summary>
    public static async Task<HcConnectedDecisionDef> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.HcConnectedDecisionDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new ConnectedDecisionNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Karar kaydı başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
}
