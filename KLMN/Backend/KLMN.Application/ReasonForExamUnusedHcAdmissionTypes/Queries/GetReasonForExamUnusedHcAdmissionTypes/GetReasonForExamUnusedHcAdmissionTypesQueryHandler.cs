using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypes;
/// <summary>Kullanılmayacak Hasta Grupları ilişkilerini arar; referansları silinmiş olabilir diye isimleri doğrudan getirir.</summary>
public sealed class GetReasonForExamUnusedHcAdmissionTypesQueryHandler(IKLMNDbContext db)
 : IRequestHandler<GetReasonForExamUnusedHcAdmissionTypesQuery,PagedResult<ReasonForExamUnusedHcAdmissionTypeResponse>>
{
 public async Task<PagedResult<ReasonForExamUnusedHcAdmissionTypeResponse>> Handle(GetReasonForExamUnusedHcAdmissionTypesQuery r,CancellationToken ct)
 {
 var q=db.ReasonForExamUnusedHcAdmissionTypes.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
 if(r.ReasonForExaminationDefId.HasValue)q=q.Where(x=>x.ReasonForExaminationDefId==r.ReasonForExaminationDefId.Value);
 if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();
 q=q.Where(x=>x.Reason.Description.ToUpper().Contains(s)||x.Reason.Code.ToUpper().Contains(s)||x.AdmissionType.Name.ToUpper().Contains(s));}
 if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
 var count=await q.CountAsync(ct);
 var items=await q.OrderBy(x=>x.Reason.Description).ThenBy(x=>x.AdmissionType.Name)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize).Select(ReasonForExamUnusedHcAdmissionTypeResponse.Projection).ToListAsync(ct);
 return new PagedResult<ReasonForExamUnusedHcAdmissionTypeResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
