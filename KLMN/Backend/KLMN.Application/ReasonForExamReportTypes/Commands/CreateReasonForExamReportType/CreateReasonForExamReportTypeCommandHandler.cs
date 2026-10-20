using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamReportTypes.Commands.Common;
using KLMN.Application.ReasonForExamReportTypes.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.CreateReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: Create işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class CreateReasonForExamReportTypeCommandHandler(IKLMNDbContext db):IRequestHandler<CreateReasonForExamReportTypeCommand,ReasonForExamReportTypeMutationResponse>
{
 public async Task<ReasonForExamReportTypeMutationResponse> Handle(CreateReasonForExamReportTypeCommand r,CancellationToken ct)
 {
 await ReasonForExamReportTypeRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct);
 await ReasonForExamReportTypeRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,null,ct);
 var item=new ReasonForExamReportType{ReasonForExaminationDefId=r.ReasonForExaminationDefId,ReportTypeDefId=r.ReferenceId};
 await db.ReasonForExamReportTypes.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
