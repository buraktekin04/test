using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.AnnotationReasons.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefs;
/// <summary>Zeyil Sebebi Tanımları verilerini filtreler, sadece mevcut sayfayı EF Core ile getirir.</summary>
public sealed class GetAnnotationReasonDefsQueryHandler(IKLMNDbContext db):
IRequestHandler<GetAnnotationReasonDefsQuery,PagedResult<AnnotationReasonDefResponse>>
{
 public async Task<PagedResult<AnnotationReasonDefResponse>> Handle(GetAnnotationReasonDefsQuery r,CancellationToken ct){
 var q=db.AnnotationReasonDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
 if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();
 q=q.Where(x=>x.Code.ToUpper().Contains(term)||x.Name.ToUpper().Contains(term));}
 if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
 var total=await q.CountAsync(ct);
 var items=await q.OrderBy(x=>x.Name).ThenBy(x=>x.Code).Skip((r.PageNumber-1)*r.PageSize)
    .Take(r.PageSize).Select(x=>new AnnotationReasonDefResponse(x.Id,x.Code,x.Name,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
 return new PagedResult<AnnotationReasonDefResponse>{Items=items,TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
