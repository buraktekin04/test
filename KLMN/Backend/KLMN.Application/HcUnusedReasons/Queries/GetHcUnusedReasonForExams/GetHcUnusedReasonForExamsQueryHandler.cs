using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedReasons.Responses;
using KLMN.Application.Common.Models;

using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExams;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedReasonForExamsQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedReasonForExamsQuery,PagedResult<HcUnusedReasonForExamResponse>>
{
 public async Task<PagedResult<HcUnusedReasonForExamResponse>> Handle(GetHcUnusedReasonForExamsQuery r,CancellationToken ct)
 {
    var q=db.HcUnusedReasonForExams.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
       if(r.HcDecisionDefId.HasValue)q=q.Where(x=>x.HcDecisionDefId==r.HcDecisionDefId.Value);
       if(!string.IsNullOrWhiteSpace(r.Search)){var term=r.Search.Trim().ToUpperInvariant();q=q.Where(x=>x.Decision.Name.ToUpper().Contains(term)||x.Reason.Description.ToUpper().Contains(term));}
       if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
       var count=await q.CountAsync(ct);
       var items=await q.OrderBy(x=>x.Decision.Name).ThenBy(x=>x.Reason.Description).Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
         .Select(x=>new HcUnusedReasonForExamResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.ReasonForExaminationDefId,x.Reason.Description,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
       return new PagedResult<HcUnusedReasonForExamResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
