using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Commands.Common;

using KLMN.Application.HcDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcDecisions.Commands.ChangeStatusHcDecisionDef;
/// <summary>Kurul kararı ChangeStatus operasyonunu iş kurallarına uygun yürütür.</summary>
public sealed class ChangeStatusHcDecisionDefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusHcDecisionDefCommand,HcDecisionDefMutationResponse>
{
    public async Task<HcDecisionDefMutationResponse> Handle(ChangeStatusHcDecisionDefCommand r,CancellationToken ct)
    {
        var item=await HcDecisionDefRules.LoadAsync(db,r.Id,r.Version,ct);
        if(!r.IsActive)await HcDecisionDefRules.EnsureNoLinksAsync(db,item.Id,true,ct);
        item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
    }
}
