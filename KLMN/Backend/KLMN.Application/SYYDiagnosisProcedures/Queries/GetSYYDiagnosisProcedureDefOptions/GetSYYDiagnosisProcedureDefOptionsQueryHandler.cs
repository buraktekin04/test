using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefOptions;
/// <summary>SYY Gereken Tetkikler: seçili pasif tarihsel kayıtlar hariç aktif referansları listeler.</summary>
public sealed class GetSYYDiagnosisProcedureDefOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisProcedureDefOptionsQuery,SYYDiagnosisProcedureDefOptions>
{
 public async Task<SYYDiagnosisProcedureDefOptions> Handle(GetSYYDiagnosisProcedureDefOptionsQuery r,CancellationToken ct)
 {
 var items0=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedSYYDiagnosisDefId))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
var items1=await db.ProcedureDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedProcedureDefId))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.ProcedureCode+" — "+x.Name,x.IsActive)).ToListAsync(ct);
 return new SYYDiagnosisProcedureDefOptions(items0,items1);
 }
}
