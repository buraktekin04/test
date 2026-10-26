using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedDecisions.Commands.Common;


using MediatR;
namespace KLMN.Application.HcUnusedDecisions.Commands.DeleteHcUnusedDecision;
/// <summary>Birlikte Verilmeyecek Kararlar için Delete işini veri bütünlüğüyle yürütür.</summary>
public sealed class DeleteHcUnusedDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteHcUnusedDecisionCommand>
{
 public async Task Handle(DeleteHcUnusedDecisionCommand r,CancellationToken ct)
 {
    var item=await HcUnusedDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
    db.HcUnusedDecisions.Remove(item);await db.SaveChangesAsync(ct);
 }
}
