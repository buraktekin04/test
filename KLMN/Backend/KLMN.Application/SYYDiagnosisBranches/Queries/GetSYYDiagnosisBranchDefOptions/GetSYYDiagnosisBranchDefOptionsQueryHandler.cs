using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefOptions;
/// <summary>SYY Gereken Branşlar: seçili pasif tarihsel kayıtlar hariç aktif referansları listeler.</summary>
public sealed class GetSYYDiagnosisBranchDefOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisBranchDefOptionsQuery,SYYDiagnosisBranchDefOptions>
{
 public async Task<SYYDiagnosisBranchDefOptions> Handle(GetSYYDiagnosisBranchDefOptionsQuery r,CancellationToken ct)
 {
 var items0=await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedSYYDiagnosisDefId))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
var items1=await db.BranchDefs.IgnoreQueryFilters().AsNoTracking()
 .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedBranchDefId))
 .OrderBy(x=>x.Name).Select(x=>new SYYReferenceOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
 return new SYYDiagnosisBranchDefOptions(items0,items1);
 }
}
