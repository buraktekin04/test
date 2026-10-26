using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedDecisions.Commands.Common;

using KLMN.Application.HcUnusedDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedDecisions.Commands.UpdateHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için Update işini veri bütünlüğüyle yürütür.</summary>
public sealed class UpdateHcUnusedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateHcUnusedDecisionCommand,HcUnusedDecisionMutationResponse>
{
 public async Task<HcUnusedDecisionMutationResponse> Handle(UpdateHcUnusedDecisionCommand r,CancellationToken ct)
 {
    var item=await HcUnusedDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
    await HcUnusedDecisionRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct,item.HcDecisionDefId,item.UnusedHcDecisionDefId);
    await HcUnusedDecisionRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,item.Id,ct);
    item.HcDecisionDefId=r.HcDecisionDefId;item.UnusedHcDecisionDefId=r.ReferenceId;
    await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
