using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Commands.Common;

using KLMN.Application.HcDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcDecisions.Commands.UpdateHcDecisionDef;
/// <summary>Kurul kararı Update operasyonunu iş kurallarına uygun yürütür.</summary>
public sealed class UpdateHcDecisionDefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateHcDecisionDefCommand,HcDecisionDefMutationResponse>
{
    public async Task<HcDecisionDefMutationResponse> Handle(UpdateHcDecisionDefCommand r,CancellationToken ct)
    {
        var item=await HcDecisionDefRules.LoadAsync(db,r.Id,r.Version,ct);
        await HcDecisionDefRules.EnsureUniqueAsync(db,r.Code,item.Id,ct);
        item.Code=r.Code;item.Name=r.Name.Trim();item.ValidFrom=r.ValidFrom;item.ValidTo=r.ValidTo;
        item.TimeInformationRequired=r.TimeInformationRequired;
item.AdditionalDescriptionRequired=r.AdditionalDescriptionRequired;
item.NegativeDecision=r.NegativeDecision;
item.ApprovalRequired=r.ApprovalRequired;
item.PertemApprovalRequired=r.PertemApprovalRequired;
item.DiagnosisRequired=r.DiagnosisRequired;
        await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
    }
}
