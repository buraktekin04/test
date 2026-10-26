using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisionConnections.Responses;
using KLMN.Application.Common.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisions;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcConnectedDecisionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcConnectedDecisionsQuery,PagedResult<HcConnectedDecisionResponse>>
{
 public async Task<PagedResult<HcConnectedDecisionResponse>> Handle(GetHcConnectedDecisionsQuery r,CancellationToken ct)
 {
    var q=db.HcConnectedDecisions.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
       if(r.HcDecisionDefId.HasValue)q=q.Where(x=>x.HcDecisionDefId==r.HcDecisionDefId.Value);
       if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();q=q.Where(x=>x.Decision.Name.ToUpper().Contains(term)||x.ConnectedDecisionDef.Name.ToUpper().Contains(term));}
       if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
       var count=await q.CountAsync(ct);
       var items=await q.OrderBy(x=>x.Decision.Name).ThenBy(x=>x.ConnectedDecisionDef.Name).Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
         .Select(x=>new HcConnectedDecisionResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.HcConnectedDecisionDefId,x.ConnectedDecisionDef.Name,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
       return new PagedResult<HcConnectedDecisionResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
