using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisBranches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefs;
/// <summary>SYY bağlantıları server-side filter/page ve ad projeksiyonu.</summary>
public sealed class GetSYYDiagnosisBranchDefsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisBranchDefsQuery,PagedResult<SYYDiagnosisBranchDefResponse>>
{
 public async Task<PagedResult<SYYDiagnosisBranchDefResponse>> Handle(GetSYYDiagnosisBranchDefsQuery r,CancellationToken ct)
 {
  var q=db.SYYDiagnosisBranchDefs.IgnoreQueryFilters().AsNoTracking()
    .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
  if(r.SYYDiagnosisDefId.HasValue)q=q.Where(x=>x.SYYDiagnosisDefId==r.SYYDiagnosisDefId.Value);
  if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();
   q=q.Where(x=>x.Diagnosis.Name.ToUpper().Contains(term)||x.Branch.Name.ToUpper().Contains(term));}
  if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
  var total=await q.CountAsync(ct);
  var items=await q.OrderBy(x=>x.Diagnosis.Name).ThenBy(x=>x.Id)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
    .Select(x=>new SYYDiagnosisBranchDefResponse(x.Id,x.SYYDiagnosisDefId,x.BranchDefId,x.Diagnosis.Name,x.Branch.Name,x.StartingAge,x.EndingAge,x.Gender,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
  return new PagedResult<SYYDiagnosisBranchDefResponse>{Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
