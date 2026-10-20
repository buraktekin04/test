using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.Common;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.CreateReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: Create işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class CreateReasonForExamUnusedHcAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<CreateReasonForExamUnusedHcAdmissionTypeCommand,ReasonForExamUnusedHcAdmissionTypeMutationResponse>
{
 public async Task<ReasonForExamUnusedHcAdmissionTypeMutationResponse> Handle(CreateReasonForExamUnusedHcAdmissionTypeCommand r,CancellationToken ct)
 {
 await ReasonForExamUnusedHcAdmissionTypeRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct);
 await ReasonForExamUnusedHcAdmissionTypeRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,null,ct);
 var item=new ReasonForExamUnusedHcAdmissionType{ReasonForExaminationDefId=r.ReasonForExaminationDefId,HcAdmissionTypeDefId=r.ReferenceId};
 await db.ReasonForExamUnusedHcAdmissionTypes.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
