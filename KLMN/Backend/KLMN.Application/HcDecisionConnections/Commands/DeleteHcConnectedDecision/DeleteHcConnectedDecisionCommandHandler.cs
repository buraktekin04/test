using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisionConnections.Commands.Common;


using MediatR;
namespace KLMN.Application.HcDecisionConnections.Commands.DeleteHcConnectedDecision;
/// <summary>Bağlı Kararlar için Delete işini veri bütünlüğüyle yürütür.</summary>
public sealed class DeleteHcConnectedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteHcConnectedDecisionCommand>
{
 public async Task Handle(DeleteHcConnectedDecisionCommand r,CancellationToken ct)
 {
    var item=await HcConnectedDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
    db.HcConnectedDecisions.Remove(item);await db.SaveChangesAsync(ct);
 }
}
