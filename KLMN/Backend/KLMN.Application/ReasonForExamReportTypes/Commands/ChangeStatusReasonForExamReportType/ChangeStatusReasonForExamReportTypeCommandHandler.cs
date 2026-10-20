using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamReportTypes.Commands.Common;
using KLMN.Application.ReasonForExamReportTypes.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.ChangeStatusReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: ChangeStatus işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class ChangeStatusReasonForExamReportTypeCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusReasonForExamReportTypeCommand,ReasonForExamReportTypeMutationResponse>
{
 public async Task<ReasonForExamReportTypeMutationResponse> Handle(ChangeStatusReasonForExamReportTypeCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamReportTypeRules.LoadAsync(db,r.Id,r.Version,ct);
 if(r.IsActive) await ReasonForExamReportTypeRules.EnsureReferencesAsync(db,item.ReasonForExaminationDefId,item.ReportTypeDefId,ct);
 item.IsActive=r.IsActive;
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
