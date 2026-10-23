using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticOptions;
/// <summary>Referansları yalnız bu modülün yetkisiyle okur; pasif seçili kayıt kaybolmaz.</summary>
public sealed class GetICD10DiagnosticOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetICD10DiagnosticOptionsQuery,ICD10DiagnosticOptions>
{
 public async Task<ICD10DiagnosticOptions> Handle(GetICD10DiagnosticOptionsQuery r,CancellationToken ct)
 {
  var icd=await db.ICD10Defs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedICD10DefId))
   .OrderBy(x=>x.Code).Select(x=>new ICD10Option(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
  var syy=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedSYYDiagnosisDefId))
   .OrderBy(x=>x.Code).Select(x=>new ICD10Option(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
  var groups=await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedHcAdmissionTypeDefId))
   .OrderBy(x=>x.Name).Select(x=>new ICD10Option(x.Id,x.Code+" — "+x.Name,x.IsActive)).ToListAsync(ct);
  return new ICD10DiagnosticOptions(icd,syy,groups);
 }
}
