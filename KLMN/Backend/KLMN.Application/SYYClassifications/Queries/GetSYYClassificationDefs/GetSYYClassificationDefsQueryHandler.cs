using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYClassifications.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefs;
/// <summary>SYY bağlantıları server-side filter/page ve ad projeksiyonu.</summary>
public sealed class GetSYYClassificationDefsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYClassificationDefsQuery,PagedResult<SYYClassificationDefResponse>>
{
 public async Task<PagedResult<SYYClassificationDefResponse>> Handle(GetSYYClassificationDefsQuery r,CancellationToken ct)
 {
  var q=db.SYYClassificationDefs.IgnoreQueryFilters().AsNoTracking()
    .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
  if(r.SYYDiagnosisDefId.HasValue)q=q.Where(x=>x.SYYDiagnosisDefId==r.SYYDiagnosisDefId.Value);
  if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();
   q=q.Where(x=>x.Diagnosis.Name.ToUpper().Contains(term)||x.ForcesCommand.Name.ToUpper().Contains(term)||x.RankGroupDefinition.Name.ToUpper().Contains(term)||x.MilitaryClassGroupDefinition.Name.ToUpper().Contains(term)||x.Decision.Name.ToUpper().Contains(term));}
  if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
  var total=await q.CountAsync(ct);
  var items=await q.OrderBy(x=>x.Diagnosis.Name).ThenBy(x=>x.Id)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
    .Select(SYYClassificationDefResponse.Projection).ToListAsync(ct);
  return new PagedResult<SYYClassificationDefResponse>{Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
