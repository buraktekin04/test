using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Commands.Common;


using MediatR;
namespace KLMN.Application.HcDecisions.Commands.DeleteHcDecisionDef;
/// <summary>Kurul kararı Delete operasyonunu iş kurallarına uygun yürütür.</summary>
public sealed class DeleteHcDecisionDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteHcDecisionDefCommand>
{
    public async Task Handle(DeleteHcDecisionDefCommand r,CancellationToken ct)
    {
        var item=await HcDecisionDefRules.LoadAsync(db,r.Id,r.Version,ct);
        await HcDecisionDefRules.EnsureNoLinksAsync(db,item.Id,false,ct);
        db.HcDecisionDefs.Remove(item);await db.SaveChangesAsync(ct);
    }
}
