using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.Common;


using MediatR;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.DeleteReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: Delete işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class DeleteReasonForExamUnusedHcAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteReasonForExamUnusedHcAdmissionTypeCommand>
{
 public async Task Handle(DeleteReasonForExamUnusedHcAdmissionTypeCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamUnusedHcAdmissionTypeRules.LoadAsync(db,r.Id,r.Version,ct);
 db.ReasonForExamUnusedHcAdmissionTypes.Remove(item);
 await db.SaveChangesAsync(ct);
 }
}
