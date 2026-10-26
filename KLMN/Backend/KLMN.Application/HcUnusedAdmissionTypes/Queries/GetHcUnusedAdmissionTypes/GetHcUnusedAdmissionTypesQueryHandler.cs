using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedAdmissionTypes.Responses;
using KLMN.Application.Common.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypes;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedAdmissionTypesQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedAdmissionTypesQuery,PagedResult<HcUnusedAdmissionTypeResponse>>
{
 public async Task<PagedResult<HcUnusedAdmissionTypeResponse>> Handle(GetHcUnusedAdmissionTypesQuery r,CancellationToken ct)
 {
    var q=db.HcUnusedAdmissionTypes.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
       if(r.HcDecisionDefId.HasValue)q=q.Where(x=>x.HcDecisionDefId==r.HcDecisionDefId.Value);
       if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();q=q.Where(x=>x.Decision.Name.ToUpper().Contains(term)||x.AdmissionType.Name.ToUpper().Contains(term));}
       if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
       var count=await q.CountAsync(ct);
       var items=await q.OrderBy(x=>x.Decision.Name).ThenBy(x=>x.AdmissionType.Name).Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
         .Select(x=>new HcUnusedAdmissionTypeResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.HcAdmissionTypeDefId,x.AdmissionType.Name,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
       return new PagedResult<HcUnusedAdmissionTypeResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
