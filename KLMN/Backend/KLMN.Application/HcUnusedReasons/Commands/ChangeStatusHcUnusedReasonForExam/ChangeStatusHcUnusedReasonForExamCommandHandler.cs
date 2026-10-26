using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedReasons.Commands.Common;

using KLMN.Application.HcUnusedReasons.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedReasons.Commands.ChangeStatusHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için ChangeStatus işini veri bütünlüğüyle yürütür.</summary>
public sealed class ChangeStatusHcUnusedReasonForExamCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusHcUnusedReasonForExamCommand,HcUnusedReasonForExamMutationResponse>
{
 public async Task<HcUnusedReasonForExamMutationResponse> Handle(ChangeStatusHcUnusedReasonForExamCommand r,CancellationToken ct)
 {
    var item=await HcUnusedReasonForExamRules.LoadAsync(db,r.Id,r.Version,ct);
    if(r.IsActive)await HcUnusedReasonForExamRules.EnsureReferencesAsync(db,item.HcDecisionDefId,item.ReasonForExaminationDefId,ct);
    item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
