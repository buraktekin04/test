using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequireds;
/// <summary>Tek tablo listesi içinde alt maddeleri birlikte döndürür, ayrı ekran gerektirmez.</summary>
public sealed class GetSYYDiagnosisRequiredsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisRequiredsQuery,PagedResult<SYYDiagnosisRequiredResponse>>
{
 public async Task<PagedResult<SYYDiagnosisRequiredResponse>> Handle(GetSYYDiagnosisRequiredsQuery r,CancellationToken ct)
 {
  var q=db.SYYDiagnosisRequireds.IgnoreQueryFilters().AsNoTracking()
    .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
  if(r.SYYDiagnosisDefId.HasValue)q=q.Where(x=>x.SYYDiagnosisDefId==r.SYYDiagnosisDefId.Value);
  if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
  if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();
   q=q.Where(x=>x.Diagnosis.Name.ToUpper().Contains(s)||
    x.Details.Any(d=>!d.IsDeleted&&d.Description.ToUpper().Contains(s)));}
  var total=await q.CountAsync(ct);
  var items=await q.OrderBy(x=>x.Diagnosis.Name).ThenBy(x=>x.Id)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
    .Select(x=>new SYYDiagnosisRequiredResponse(x.Id,x.SYYDiagnosisDefId,x.Diagnosis.Name,
 x.MustAll,x.MustOneOf,x.IsActive,x.Version,
 x.Details.Where(d=>!d.IsDeleted).OrderBy(d=>d.SortOrder)
   .Select(d=>new RequiredDetailResponse(d.Id,d.Description,d.SortOrder)).ToList())).ToListAsync(ct);
  return new PagedResult<SYYDiagnosisRequiredResponse>
  {Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
