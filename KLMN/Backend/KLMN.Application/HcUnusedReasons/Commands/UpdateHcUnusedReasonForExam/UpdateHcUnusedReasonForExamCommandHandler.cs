using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedReasons.Commands.Common;

using KLMN.Application.HcUnusedReasons.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedReasons.Commands.UpdateHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için Update işini veri bütünlüğüyle yürütür.</summary>
public sealed class UpdateHcUnusedReasonForExamCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateHcUnusedReasonForExamCommand,HcUnusedReasonForExamMutationResponse>
{
 public async Task<HcUnusedReasonForExamMutationResponse> Handle(UpdateHcUnusedReasonForExamCommand r,CancellationToken ct)
 {
    var item=await HcUnusedReasonForExamRules.LoadAsync(db,r.Id,r.Version,ct);
    await HcUnusedReasonForExamRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct,item.HcDecisionDefId,item.ReasonForExaminationDefId);
    await HcUnusedReasonForExamRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,item.Id,ct);
    item.HcDecisionDefId=r.HcDecisionDefId;item.ReasonForExaminationDefId=r.ReferenceId;
    await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
