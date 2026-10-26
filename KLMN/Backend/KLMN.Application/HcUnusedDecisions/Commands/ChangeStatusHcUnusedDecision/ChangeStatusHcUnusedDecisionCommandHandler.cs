using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedDecisions.Commands.Common;

using KLMN.Application.HcUnusedDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedDecisions.Commands.ChangeStatusHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için ChangeStatus işini veri bütünlüğüyle yürütür.</summary>
public sealed class ChangeStatusHcUnusedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusHcUnusedDecisionCommand,HcUnusedDecisionMutationResponse>
{
 public async Task<HcUnusedDecisionMutationResponse> Handle(ChangeStatusHcUnusedDecisionCommand r,CancellationToken ct)
 {
    var item=await HcUnusedDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
    if(r.IsActive)await HcUnusedDecisionRules.EnsureReferencesAsync(db,item.HcDecisionDefId,item.UnusedHcDecisionDefId,ct);
    item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
