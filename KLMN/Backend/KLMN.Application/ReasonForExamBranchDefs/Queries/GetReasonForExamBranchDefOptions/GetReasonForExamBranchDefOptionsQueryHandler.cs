using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefOptions;
/// <summary>Yeni seçimde aktif kayıtları, düzenlemede seçili pasif kaydı getirir.</summary>
public sealed class GetReasonForExamBranchDefOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetReasonForExamBranchDefOptionsQuery,ReasonRelationOptions>
{
 public async Task<ReasonRelationOptions> Handle(GetReasonForExamBranchDefOptionsQuery r,CancellationToken ct)
 {
 var reasons=await db.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedReasonId))
 .OrderBy(x=>x.Description).Select(x=>new ReasonOption(x.Id,x.Code+" - "+x.Description,x.IsActive)).ToListAsync(ct);
 var refs=await db.BranchDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedReferenceId))
 .OrderBy(x=>x.Name).Select(x=>new ReasonOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
 return new ReasonRelationOptions(reasons,refs);
 }
}
