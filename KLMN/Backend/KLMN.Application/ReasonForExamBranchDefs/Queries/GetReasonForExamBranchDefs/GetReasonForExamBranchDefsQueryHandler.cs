using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefs;
/// <summary>Muayene Gereken Branşlar ilişkilerini arar; referansları silinmiş olabilir diye isimleri doğrudan getirir.</summary>
public sealed class GetReasonForExamBranchDefsQueryHandler(IKLMNDbContext db)
 : IRequestHandler<GetReasonForExamBranchDefsQuery,PagedResult<ReasonForExamBranchDefResponse>>
{
 public async Task<PagedResult<ReasonForExamBranchDefResponse>> Handle(GetReasonForExamBranchDefsQuery r,CancellationToken ct)
 {
 var q=db.ReasonForExamBranchDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
 if(r.ReasonForExaminationDefId.HasValue)q=q.Where(x=>x.ReasonForExaminationDefId==r.ReasonForExaminationDefId.Value);
 if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();
 q=q.Where(x=>x.Reason.Description.ToUpper().Contains(s)||x.Reason.Code.ToUpper().Contains(s)||x.Branch.Name.ToUpper().Contains(s));}
 if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
 var count=await q.CountAsync(ct);
 var items=await q.OrderBy(x=>x.Reason.Description).ThenBy(x=>x.Branch.Name)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize).Select(ReasonForExamBranchDefResponse.Projection).ToListAsync(ct);
 return new PagedResult<ReasonForExamBranchDefResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
