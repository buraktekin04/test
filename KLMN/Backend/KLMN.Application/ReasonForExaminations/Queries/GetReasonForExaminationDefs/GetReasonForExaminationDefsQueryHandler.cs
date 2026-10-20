using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExaminations.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefs;
/// <summary>Muayene sebeplerini sunucuda sayfalar; dört ilişki listesini yalnız sayfadaki kayıtlara yükler.</summary>
public sealed class GetReasonForExaminationDefsQueryHandler(IKLMNDbContext db)
 : IRequestHandler<GetReasonForExaminationDefsQuery,PagedResult<ReasonForExaminationDefResponse>>
{
 public async Task<PagedResult<ReasonForExaminationDefResponse>> Handle(GetReasonForExaminationDefsQuery r,CancellationToken ct)
 {
   var q=db.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
   if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();q=q.Where(x=>x.Code.ToUpper().Contains(s)||x.Description.ToUpper().Contains(s));}
   if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
   var total=await q.CountAsync(ct);
   var items=await q.OrderBy(x=>x.Description).ThenBy(x=>x.Code)
     .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize).Include(x=>x.ReportTypes).ThenInclude(x=>x.ReportType)
            .Include(x=>x.UnusedAdmissionTypes).ThenInclude(x=>x.AdmissionType)
            .Include(x=>x.Procedures).ThenInclude(x=>x.Procedure)
            .Include(x=>x.Branches).ThenInclude(x=>x.Branch).AsSplitQuery().ToListAsync(ct);
   return new PagedResult<ReasonForExaminationDefResponse>{Items=items.Select(ReasonForExaminationDefResponse.From).ToList(),
     TotalCount=total,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
