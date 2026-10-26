using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisionOptions;
/// <summary>SYY Tanı Kodu, Kurul Kararı, Başvuru Grubu ve Askerî Sınıf dropdown'ları.</summary>
public sealed class GetSYYDiagnosisDecisionOptionsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisDecisionOptionsQuery,SYYDiagnosisDecisionOptionsResponse>
{
 public async Task<SYYDiagnosisDecisionOptionsResponse> Handle(GetSYYDiagnosisDecisionOptionsQuery r,CancellationToken ct)
 {
  var syy=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(r.IncludeInactive||x.IsActive||x.Id==r.SelectedSYYDiagnosisDefId))
   .OrderBy(x=>x.Code).Select(x=>new SYYDiagnosisDecisionOption(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
  var decisions=await db.HcDecisionDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(r.IncludeInactive||x.IsActive||x.Id==r.SelectedHcDecisionDefId))
   .OrderBy(x=>x.Code).Select(x=>new SYYDiagnosisDecisionOption(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
  var admissions=await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(r.IncludeInactive||x.IsActive||x.Id==r.SelectedHcAdmissionTypeDefId))
   .OrderBy(x=>x.Code).Select(x=>new SYYDiagnosisDecisionOption(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
  var classes=await db.MilitaryClassDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(r.IncludeInactive||x.IsActive||x.Id==r.SelectedMilitaryClassDefId))
   .OrderBy(x=>x.Code).Select(x=>new SYYDiagnosisDecisionOption(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
  return new SYYDiagnosisDecisionOptionsResponse(syy,decisions,admissions,classes);
 }
}
