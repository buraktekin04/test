using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedDecisions.Commands.Common;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.HcUnusedDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedDecisions.Commands.CreateHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için Create işini veri bütünlüğüyle yürütür.</summary>
public sealed class CreateHcUnusedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<CreateHcUnusedDecisionCommand,HcUnusedDecisionMutationResponse>
{
 public async Task<HcUnusedDecisionMutationResponse> Handle(CreateHcUnusedDecisionCommand r,CancellationToken ct)
 {
    await HcUnusedDecisionRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct);
    await HcUnusedDecisionRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,null,ct);
    var item=new HcUnusedDecision{HcDecisionDefId=r.HcDecisionDefId,UnusedHcDecisionDefId=r.ReferenceId};
    await db.HcUnusedDecisions.AddAsync(item,ct);await db.SaveChangesAsync(ct);
    return new(item.Id,item.Version);
 }
}
