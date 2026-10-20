using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamBranchDefs.Commands.Common;


using MediatR;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.DeleteReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: Delete işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class DeleteReasonForExamBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteReasonForExamBranchDefCommand>
{
 public async Task Handle(DeleteReasonForExamBranchDefCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamBranchDefRules.LoadAsync(db,r.Id,r.Version,ct);
 db.ReasonForExamBranchDefs.Remove(item);
 await db.SaveChangesAsync(ct);
 }
}
