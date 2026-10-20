using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypeOptions;
/// <summary>Yeni seçimde aktif kayıtları, düzenlemede seçili pasif kaydı getirir.</summary>
public sealed class GetReasonForExamUnusedHcAdmissionTypeOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetReasonForExamUnusedHcAdmissionTypeOptionsQuery,ReasonRelationOptions>
{
 public async Task<ReasonRelationOptions> Handle(GetReasonForExamUnusedHcAdmissionTypeOptionsQuery r,CancellationToken ct)
 {
 var reasons=await db.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedReasonId))
 .OrderBy(x=>x.Description).Select(x=>new ReasonOption(x.Id,x.Code+" - "+x.Description,x.IsActive)).ToListAsync(ct);
 var refs=await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedReferenceId))
 .OrderBy(x=>x.Name).Select(x=>new ReasonOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
 return new ReasonRelationOptions(reasons,refs);
 }
}
