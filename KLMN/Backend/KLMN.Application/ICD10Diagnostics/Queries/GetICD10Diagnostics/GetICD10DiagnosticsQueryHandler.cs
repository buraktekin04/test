using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.ICD10Diagnostics.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10Diagnostics;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: EF projection, server filtering/paging.</summary>
public sealed class GetICD10DiagnosticsQueryHandler(IKLMNDbContext db):IRequestHandler<GetICD10DiagnosticsQuery,PagedResult<ICD10DiagnosticResponse>>
{
 public async Task<PagedResult<ICD10DiagnosticResponse>> Handle(GetICD10DiagnosticsQuery r,CancellationToken ct)
 {
  var q=db.ICD10Diagnostics.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
  
 if(r.ICD10DefId.HasValue)q=q.Where(x=>x.ICD10DefId==r.ICD10DefId.Value);
 if(r.SYYDiagnosisDefId.HasValue)q=q.Where(x=>x.SYYDiagnosisDefId==r.SYYDiagnosisDefId.Value);
 if(r.HcAdmissionTypeDefId.HasValue)q=q.Where(x=>x.HcAdmissionTypeDefId==r.HcAdmissionTypeDefId.Value);

  if(!string.IsNullOrWhiteSpace(r.Search)){var s=r.Search.Trim().ToUpperInvariant();
   q=q.Where(x=>x.ICD10.Code.ToUpper().Contains(s)||x.ICD10.Name.ToUpper().Contains(s)||
 x.SYYDiagnosis.Name.ToUpper().Contains(s)||x.SYYDiagnosis.Code.ToUpper().Contains(s)||
 x.AdmissionType.Name.ToUpper().Contains(s));}
  if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
  var count=await q.CountAsync(ct);
  var items=await q.OrderBy(x=>x.ICD10.Code).ThenBy(x=>x.Id)
   .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
   .Select(x=>
 new ICD10DiagnosticResponse(x.Id,x.ICD10DefId,x.ICD10.Code,x.ICD10.Name,x.SYYDiagnosisDefId,x.SYYDiagnosis.Code+" — "+x.SYYDiagnosis.Name,x.HcAdmissionTypeDefId,x.AdmissionType.Name,x.Amount,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
  return new PagedResult<ICD10DiagnosticResponse>{Items=items,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
