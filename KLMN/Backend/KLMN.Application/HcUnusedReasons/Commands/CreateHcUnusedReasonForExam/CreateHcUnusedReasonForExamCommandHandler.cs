using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedReasons.Commands.Common;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.HcUnusedReasons.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedReasons.Commands.CreateHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için Create işini veri bütünlüğüyle yürütür.</summary>
public sealed class CreateHcUnusedReasonForExamCommandHandler(IKLMNDbContext db):IRequestHandler<CreateHcUnusedReasonForExamCommand,HcUnusedReasonForExamMutationResponse>
{
 public async Task<HcUnusedReasonForExamMutationResponse> Handle(CreateHcUnusedReasonForExamCommand r,CancellationToken ct)
 {
    await HcUnusedReasonForExamRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct);
    await HcUnusedReasonForExamRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,null,ct);
    var item=new HcUnusedReasonForExam{HcDecisionDefId=r.HcDecisionDefId,ReasonForExaminationDefId=r.ReferenceId};
    await db.HcUnusedReasonForExams.AddAsync(item,ct);await db.SaveChangesAsync(ct);
    return new(item.Id,item.Version);
 }
}
