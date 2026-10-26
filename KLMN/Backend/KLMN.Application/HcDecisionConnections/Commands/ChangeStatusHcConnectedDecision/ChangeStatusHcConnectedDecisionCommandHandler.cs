using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisionConnections.Commands.Common;

using KLMN.Application.HcDecisionConnections.Responses;
using MediatR;
namespace KLMN.Application.HcDecisionConnections.Commands.ChangeStatusHcConnectedDecision;
/// <summary>Bağlı Kararlar için ChangeStatus işini veri bütünlüğüyle yürütür.</summary>
public sealed class ChangeStatusHcConnectedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusHcConnectedDecisionCommand,HcConnectedDecisionMutationResponse>
{
 public async Task<HcConnectedDecisionMutationResponse> Handle(ChangeStatusHcConnectedDecisionCommand r,CancellationToken ct)
 {
    var item=await HcConnectedDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
    if(r.IsActive)await HcConnectedDecisionRules.EnsureReferencesAsync(db,item.HcDecisionDefId,item.HcConnectedDecisionDefId,ct);
    item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
