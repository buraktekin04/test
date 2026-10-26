using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedDecisions.Responses;
using KLMN.Application.Common.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisions;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedDecisionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedDecisionsQuery,PagedResult<HcUnusedDecisionResponse>>
{
 public async Task<PagedResult<HcUnusedDecisionResponse>> Handle(GetHcUnusedDecisionsQuery r,CancellationToken ct)
 {
    var q=db.HcUnusedDecisions.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
       if(r.HcDecisionDefId.HasValue)q=q.Where(x=>x.HcDecisionDefId==r.HcDecisionDefId.Value);
       if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();q=q.Where(x=>x.Decision.Name.ToUpper().Contains(term)||x.UnusedDecision.Name.ToUpper().Contains(term));}
       if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
       var count=await q.CountAsync(ct);
       var items=await q.OrderBy(x=>x.Decision.Name).ThenBy(x=>x.UnusedDecision.Name).Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
         .Select(x=>new HcUnusedDecisionResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.UnusedHcDecisionDefId,x.UnusedDecision.Name,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
       return new PagedResult<HcUnusedDecisionResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
