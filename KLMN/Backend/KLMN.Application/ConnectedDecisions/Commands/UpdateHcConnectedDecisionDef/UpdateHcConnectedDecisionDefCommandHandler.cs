using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Commands.Common;
using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.UpdateHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateHcConnectedDecisionDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateHcConnectedDecisionDefCommand, HcConnectedDecisionDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HcConnectedDecisionDefMutationResponse> Handle(UpdateHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        var item = await HcConnectedDecisionDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await HcConnectedDecisionDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new HcConnectedDecisionDefMutationResponse(item.Id, item.Version);
    }
}
