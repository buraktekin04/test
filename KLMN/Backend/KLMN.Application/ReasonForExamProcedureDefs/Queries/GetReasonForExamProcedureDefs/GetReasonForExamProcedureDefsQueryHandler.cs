using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefs;
/// <summary>Gereken Tetkikler ilişkilerini arar; referansları silinmiş olabilir diye isimleri doğrudan getirir.</summary>
public sealed class GetReasonForExamProcedureDefsQueryHandler(IKLMNDbContext db)
 : IRequestHandler<GetReasonForExamProcedureDefsQuery,PagedResult<ReasonForExamProcedureDefResponse>>
{
 public async Task<PagedResult<ReasonForExamProcedureDefResponse>> Handle(GetReasonForExamProcedureDefsQuery r,CancellationToken ct)
 {
 var q=db.ReasonForExamProcedureDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
 if(r.ReasonForExaminationDefId.HasValue)q=q.Where(x=>x.ReasonForExaminationDefId==r.ReasonForExaminationDefId.Value);
 if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();
 q=q.Where(x=>x.Reason.Description.ToUpper().Contains(s)||x.Reason.Code.ToUpper().Contains(s)||x.Procedure.Name.ToUpper().Contains(s));}
 if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
 var count=await q.CountAsync(ct);
 var items=await q.OrderBy(x=>x.Reason.Description).ThenBy(x=>x.Procedure.Name)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize).Select(x=>new ReasonForExamProcedureDefResponse(
 x.Id,x.ReasonForExaminationDefId,x.Reason.Description,x.Reason.Code,
 x.ProcedureDefId,x.Procedure.Name,x.IsActive,x.Version,x.CreatedDate
 ,x.StartingAge,x.EndingAge,x.Gender)).ToListAsync(ct);
 return new PagedResult<ReasonForExamProcedureDefResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
