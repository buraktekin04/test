using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisionConnections.Commands.Common;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.HcDecisionConnections.Responses;
using MediatR;
namespace KLMN.Application.HcDecisionConnections.Commands.CreateHcConnectedDecision;
/// <summary>Bağlı Kararlar için Create işini veri bütünlüğüyle yürütür.</summary>
public sealed class CreateHcConnectedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<CreateHcConnectedDecisionCommand,HcConnectedDecisionMutationResponse>
{
 public async Task<HcConnectedDecisionMutationResponse> Handle(CreateHcConnectedDecisionCommand r,CancellationToken ct)
 {
    await HcConnectedDecisionRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct);
    await HcConnectedDecisionRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,null,ct);
    var item=new HcConnectedDecision{HcDecisionDefId=r.HcDecisionDefId,HcConnectedDecisionDefId=r.ReferenceId};
    await db.HcConnectedDecisions.AddAsync(item,ct);await db.SaveChangesAsync(ct);
    return new(item.Id,item.Version);
 }
}
