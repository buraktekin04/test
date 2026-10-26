using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Commands.Common;
using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.ConnectedDecisions.Exceptions;

namespace KLMN.Application.ConnectedDecisions.Commands.ChangeStatusHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusHcConnectedDecisionDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusHcConnectedDecisionDefCommand, HcConnectedDecisionDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HcConnectedDecisionDefMutationResponse> Handle(ChangeStatusHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        var item = await HcConnectedDecisionDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if(!request.IsActive && await db.HcConnectedDecisions.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.HcConnectedDecisionDefId==item.Id,ct))
            throw new ConnectedDecisionConflictException("Kurul kararına bağlı aktif eşleştirme bulunuyor.");
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new HcConnectedDecisionDefMutationResponse(item.Id, item.Version);
    }
}
