using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.Common;


using MediatR;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.DeleteReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: Delete işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class DeleteReasonForExamProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteReasonForExamProcedureDefCommand>
{
 public async Task Handle(DeleteReasonForExamProcedureDefCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamProcedureDefRules.LoadAsync(db,r.Id,r.Version,ct);
 db.ReasonForExamProcedureDefs.Remove(item);
 await db.SaveChangesAsync(ct);
 }
}
