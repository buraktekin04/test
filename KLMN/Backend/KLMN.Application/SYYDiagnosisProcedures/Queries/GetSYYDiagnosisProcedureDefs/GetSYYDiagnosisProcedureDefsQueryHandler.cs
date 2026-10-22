using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefs;
/// <summary>SYY bağlantıları server-side filter/page ve ad projeksiyonu.</summary>
public sealed class GetSYYDiagnosisProcedureDefsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisProcedureDefsQuery,PagedResult<SYYDiagnosisProcedureDefResponse>>
{
 public async Task<PagedResult<SYYDiagnosisProcedureDefResponse>> Handle(GetSYYDiagnosisProcedureDefsQuery r,CancellationToken ct)
 {
  var q=db.SYYDiagnosisProcedureDefs.IgnoreQueryFilters().AsNoTracking()
    .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
  if(r.SYYDiagnosisDefId.HasValue)q=q.Where(x=>x.SYYDiagnosisDefId==r.SYYDiagnosisDefId.Value);
  if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();
   q=q.Where(x=>x.Diagnosis.Name.ToUpper().Contains(term)||x.Procedure.Name.ToUpper().Contains(term));}
  if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
  var total=await q.CountAsync(ct);
  var items=await q.OrderBy(x=>x.Diagnosis.Name).ThenBy(x=>x.Id)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
    .Select(SYYDiagnosisProcedureDefResponse.Projection).ToListAsync(ct);
  return new PagedResult<SYYDiagnosisProcedureDefResponse>{Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
