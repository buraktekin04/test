using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Commands.Common;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.HcDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcDecisions.Commands.CreateHcDecisionDef;
/// <summary>Kurul kararı Create operasyonunu iş kurallarına uygun yürütür.</summary>
public sealed class CreateHcDecisionDefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateHcDecisionDefCommand,HcDecisionDefMutationResponse>
{
    public async Task<HcDecisionDefMutationResponse> Handle(CreateHcDecisionDefCommand r,CancellationToken ct)
    {
        await HcDecisionDefRules.EnsureUniqueAsync(db,r.Code,null,ct);
        var item=new HcDecisionDef{ Code=r.Code,Name=r.Name.Trim(),ValidFrom=r.ValidFrom,ValidTo=r.ValidTo };
        item.TimeInformationRequired=r.TimeInformationRequired;
item.AdditionalDescriptionRequired=r.AdditionalDescriptionRequired;
item.NegativeDecision=r.NegativeDecision;
item.ApprovalRequired=r.ApprovalRequired;
item.PertemApprovalRequired=r.PertemApprovalRequired;
item.DiagnosisRequired=r.DiagnosisRequired;
        await db.HcDecisionDefs.AddAsync(item,ct);await db.SaveChangesAsync(ct);
        return new(item.Id,item.Version);
    }
}
