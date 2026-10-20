using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.Common;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.UpdateReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: Update işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class UpdateReasonForExamUnusedHcAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateReasonForExamUnusedHcAdmissionTypeCommand,ReasonForExamUnusedHcAdmissionTypeMutationResponse>
{
 public async Task<ReasonForExamUnusedHcAdmissionTypeMutationResponse> Handle(UpdateReasonForExamUnusedHcAdmissionTypeCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamUnusedHcAdmissionTypeRules.LoadAsync(db,r.Id,r.Version,ct);
 await ReasonForExamUnusedHcAdmissionTypeRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct,item.ReasonForExaminationDefId,item.HcAdmissionTypeDefId);
 await ReasonForExamUnusedHcAdmissionTypeRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,item.Id,ct);
 item.ReasonForExaminationDefId=r.ReasonForExaminationDefId;item.HcAdmissionTypeDefId=r.ReferenceId;
 
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
