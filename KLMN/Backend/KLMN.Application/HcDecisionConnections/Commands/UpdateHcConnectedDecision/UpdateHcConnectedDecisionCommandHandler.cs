using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisionConnections.Commands.Common;

using KLMN.Application.HcDecisionConnections.Responses;
using MediatR;
namespace KLMN.Application.HcDecisionConnections.Commands.UpdateHcConnectedDecision;
/// <summary>Bağlı Kararlar için Update işini veri bütünlüğüyle yürütür.</summary>
public sealed class UpdateHcConnectedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateHcConnectedDecisionCommand,HcConnectedDecisionMutationResponse>
{
 public async Task<HcConnectedDecisionMutationResponse> Handle(UpdateHcConnectedDecisionCommand r,CancellationToken ct)
 {
    var item=await HcConnectedDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
    await HcConnectedDecisionRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct,item.HcDecisionDefId,item.HcConnectedDecisionDefId);
    await HcConnectedDecisionRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,item.Id,ct);
    item.HcDecisionDefId=r.HcDecisionDefId;item.HcConnectedDecisionDefId=r.ReferenceId;
    await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
