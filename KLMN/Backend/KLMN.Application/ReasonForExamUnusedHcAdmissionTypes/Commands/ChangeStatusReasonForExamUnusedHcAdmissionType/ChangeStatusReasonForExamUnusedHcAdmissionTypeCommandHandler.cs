using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.Common;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.ChangeStatusReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: ChangeStatus işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class ChangeStatusReasonForExamUnusedHcAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusReasonForExamUnusedHcAdmissionTypeCommand,ReasonForExamUnusedHcAdmissionTypeMutationResponse>
{
 public async Task<ReasonForExamUnusedHcAdmissionTypeMutationResponse> Handle(ChangeStatusReasonForExamUnusedHcAdmissionTypeCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamUnusedHcAdmissionTypeRules.LoadAsync(db,r.Id,r.Version,ct);
 if(r.IsActive) await ReasonForExamUnusedHcAdmissionTypeRules.EnsureReferencesAsync(db,item.ReasonForExaminationDefId,item.HcAdmissionTypeDefId,ct);
 item.IsActive=r.IsActive;
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
