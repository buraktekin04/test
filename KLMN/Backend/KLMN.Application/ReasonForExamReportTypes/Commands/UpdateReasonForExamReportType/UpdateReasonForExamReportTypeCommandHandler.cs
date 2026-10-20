using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamReportTypes.Commands.Common;
using KLMN.Application.ReasonForExamReportTypes.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.UpdateReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: Update işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class UpdateReasonForExamReportTypeCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateReasonForExamReportTypeCommand,ReasonForExamReportTypeMutationResponse>
{
 public async Task<ReasonForExamReportTypeMutationResponse> Handle(UpdateReasonForExamReportTypeCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamReportTypeRules.LoadAsync(db,r.Id,r.Version,ct);
 await ReasonForExamReportTypeRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct,item.ReasonForExaminationDefId,item.ReportTypeDefId);
 await ReasonForExamReportTypeRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,item.Id,ct);
 item.ReasonForExaminationDefId=r.ReasonForExaminationDefId;item.ReportTypeDefId=r.ReferenceId;
 
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
