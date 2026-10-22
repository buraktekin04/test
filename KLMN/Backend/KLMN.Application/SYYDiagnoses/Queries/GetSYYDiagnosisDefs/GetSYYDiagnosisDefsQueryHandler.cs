using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnoses.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefs;
/// <summary>SYY Tanı Listesi (M/D/F) verilerini filtreler, sadece mevcut sayfayı EF Core ile getirir.</summary>
public sealed class GetSYYDiagnosisDefsQueryHandler(IKLMNDbContext db):
IRequestHandler<GetSYYDiagnosisDefsQuery,PagedResult<SYYDiagnosisDefResponse>>
{
 public async Task<PagedResult<SYYDiagnosisDefResponse>> Handle(GetSYYDiagnosisDefsQuery r,CancellationToken ct){
 var q=db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
 if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();
 q=q.Where(x=>x.Code.ToUpper().Contains(term)||x.Name.ToUpper().Contains(term));}
 if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
 var total=await q.CountAsync(ct);
 var items=await q.OrderBy(x=>x.Name).ThenBy(x=>x.Code).Skip((r.PageNumber-1)*r.PageSize)
    .Take(r.PageSize).Select(SYYDiagnosisDefResponse.Projection).ToListAsync(ct);
 return new PagedResult<SYYDiagnosisDefResponse>{Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
