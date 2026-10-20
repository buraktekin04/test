using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamReportTypes.Commands.Common;


using MediatR;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.DeleteReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: Delete işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class DeleteReasonForExamReportTypeCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteReasonForExamReportTypeCommand>
{
 public async Task Handle(DeleteReasonForExamReportTypeCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamReportTypeRules.LoadAsync(db,r.Id,r.Version,ct);
 db.ReasonForExamReportTypes.Remove(item);
 await db.SaveChangesAsync(ct);
 }
}
