using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisTypes.Queries.GetSYYDiagnosisTypeDefs;
/// <summary>SYY Tanı Türü Tanımları verilerini filtreler, sadece mevcut sayfayı EF Core ile getirir.</summary>
public sealed class GetSYYDiagnosisTypeDefsQueryHandler(IKLMNDbContext db):
IRequestHandler<GetSYYDiagnosisTypeDefsQuery,PagedResult<SYYDiagnosisTypeDefResponse>>
{
 public async Task<PagedResult<SYYDiagnosisTypeDefResponse>> Handle(GetSYYDiagnosisTypeDefsQuery r,CancellationToken ct){
 var q=db.SYYDiagnosisTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
 if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();
 q=q.Where(x=>x.Name.ToUpper().Contains(term)||x.Code.ToString()==term);}
 if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
 var total=await q.CountAsync(ct);
 var items=await q.OrderBy(x=>x.Name).ThenBy(x=>x.Code).Skip((r.PageNumber-1)*r.PageSize)
    .Take(r.PageSize).Select(SYYDiagnosisTypeDefResponse.Projection).ToListAsync(ct);
 return new PagedResult<SYYDiagnosisTypeDefResponse>{Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
