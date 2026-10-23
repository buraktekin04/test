using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ICD10Defs.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ICD10Defs.Queries.GetICD10Defs;
/// <summary>ICD10 Tanımları: EF projection, server filtering/paging.</summary>
public sealed class GetICD10DefsQueryHandler(IKLMNDbContext db):IRequestHandler<GetICD10DefsQuery,PagedResult<ICD10DefResponse>>
{
 public async Task<PagedResult<ICD10DefResponse>> Handle(GetICD10DefsQuery r,CancellationToken ct)
 {
  var q=db.ICD10Defs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
  
  if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();
   q=q.Where(x=>x.Code.ToUpper().Contains(s)||x.Name.ToUpper().Contains(s));}
  if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
  var count=await q.CountAsync(ct);
  var items=await q.OrderBy(x=>x.Code).ThenBy(x=>x.Id)
   .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
   .Select(x=>
 new ICD10DefResponse(x.Id,x.Code,x.Name,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
  return new PagedResult<ICD10DefResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
