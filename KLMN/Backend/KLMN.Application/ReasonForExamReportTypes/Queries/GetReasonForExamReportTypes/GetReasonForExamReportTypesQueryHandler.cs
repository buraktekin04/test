using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamReportTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypes;
/// <summary>Muayene Sebebi Rapor Türleri ilişkilerini arar; referansları silinmiş olabilir diye isimleri doğrudan getirir.</summary>
public sealed class GetReasonForExamReportTypesQueryHandler(IKLMNDbContext db)
 : IRequestHandler<GetReasonForExamReportTypesQuery,PagedResult<ReasonForExamReportTypeResponse>>
{
 public async Task<PagedResult<ReasonForExamReportTypeResponse>> Handle(GetReasonForExamReportTypesQuery r,CancellationToken ct)
 {
 var q=db.ReasonForExamReportTypes.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
 if(r.ReasonForExaminationDefId.HasValue)q=q.Where(x=>x.ReasonForExaminationDefId==r.ReasonForExaminationDefId.Value);
 if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();
 q=q.Where(x=>x.Reason.Description.ToUpper().Contains(s)||x.Reason.Code.ToUpper().Contains(s)||x.ReportType.Name.ToUpper().Contains(s));}
 if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
 var count=await q.CountAsync(ct);
 var items=await q.OrderBy(x=>x.Reason.Description).ThenBy(x=>x.ReportType.Name)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize).Select(ReasonForExamReportTypeResponse.Projection).ToListAsync(ct);
 return new PagedResult<ReasonForExamReportTypeResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
