using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.HcDecisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcDecisions.Queries.GetHcDecisionDefs;
/// <summary>Kararları sunucuda sorgular; alt ilişki adları yalnız getirilen sayfaya yüklenir.</summary>
public sealed class GetHcDecisionDefsQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcDecisionDefsQuery,PagedResult<HcDecisionDefResponse>>
{
    public async Task<PagedResult<HcDecisionDefResponse>> Handle(GetHcDecisionDefsQuery r,CancellationToken ct)
    {
        var q=db.HcDecisionDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
        if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();q=q.Where(x=>x.Name.ToUpper().Contains(s)||x.Code.ToString().Contains(s));}
        if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
        var total=await q.CountAsync(ct);
        var items=await q.OrderBy(x=>x.Code).ThenBy(x=>x.Name).Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
           .Include(x=>x.UnusedReasons).ThenInclude(x=>x.Reason)
 .Include(x=>x.UnusedAdmissionTypes).ThenInclude(x=>x.AdmissionType)
 .Include(x=>x.UnusedDecisions).ThenInclude(x=>x.UnusedDecision)
 .Include(x=>x.ConnectedDecisions).ThenInclude(x=>x.ConnectedDecisionDef).AsSplitQuery().ToListAsync(ct);
        return new PagedResult<HcDecisionDefResponse>{Items=items.Select(HcDecisionDefResponse.From).ToList(),TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
    }
}
