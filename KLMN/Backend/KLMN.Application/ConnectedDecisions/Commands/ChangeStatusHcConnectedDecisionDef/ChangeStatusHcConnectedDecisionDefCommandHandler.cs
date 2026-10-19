using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Commands.Common;
using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.ChangeStatusHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusHcConnectedDecisionDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusHcConnectedDecisionDefCommand, HcConnectedDecisionDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HcConnectedDecisionDefMutationResponse> Handle(ChangeStatusHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        var item = await HcConnectedDecisionDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new HcConnectedDecisionDefMutationResponse(item.Id, item.Version);
    }
}
