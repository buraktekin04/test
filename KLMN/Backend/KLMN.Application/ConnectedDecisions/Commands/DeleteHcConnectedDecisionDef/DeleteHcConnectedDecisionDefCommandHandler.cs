using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Commands.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.ConnectedDecisions.Exceptions;

namespace KLMN.Application.ConnectedDecisions.Commands.DeleteHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteHcConnectedDecisionDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteHcConnectedDecisionDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        var item = await HcConnectedDecisionDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        // Bağlı karar tanımı, kurul kararları tarafından kullanılıyorsa silinmez.
        if(await db.HcConnectedDecisions.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.HcConnectedDecisionDefId==item.Id,ct))
            throw new ConnectedDecisionConflictException("Kurul kararına bağlı silinmemiş eşleştirme bulunuyor.");
        db.HcConnectedDecisionDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
