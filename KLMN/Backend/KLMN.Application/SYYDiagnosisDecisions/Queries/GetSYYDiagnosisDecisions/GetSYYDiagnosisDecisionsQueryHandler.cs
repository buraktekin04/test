using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisions;
/// <summary>Üç dropdown FK filtresini birlikte uygular; projection yalnız Handler'dadır.</summary>
public sealed class GetSYYDiagnosisDecisionsQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisDecisionsQuery,PagedResult<SYYDiagnosisDecisionResponse>>
{
 public async Task<PagedResult<SYYDiagnosisDecisionResponse>> Handle(GetSYYDiagnosisDecisionsQuery r,CancellationToken ct)
 {
   var q=db.SYYDiagnosisDecisions.IgnoreQueryFilters().AsNoTracking()
      .Where(x=>!x.IsDeleted&&(r.IncludeInactive||r.IsActive==false||x.IsActive));
   if(r.IsActive.HasValue)q=q.Where(x=>x.IsActive==r.IsActive.Value);
   if(r.SYYDiagnosisDefId.HasValue)q=q.Where(x=>x.SYYDiagnosisDefId==r.SYYDiagnosisDefId.Value);
   if(r.HcAdmissionTypeDefId.HasValue)q=q.Where(x=>x.HcAdmissionTypeDefId==r.HcAdmissionTypeDefId.Value);
   if(r.MilitaryClassDefId.HasValue)q=q.Where(x=>x.MilitaryClassDefId==r.MilitaryClassDefId.Value);
   if(r.HcDecisionDefId.HasValue)q=q.Where(x=>x.HcDecisionDefId==r.HcDecisionDefId.Value);
   if(!string.IsNullOrWhiteSpace(r.Search))
   {
      var s=r.Search.Trim().ToUpperInvariant();
      q=q.Where(x=>x.SYYDiagnosis.Code.ToUpper().Contains(s)||
       x.SYYDiagnosis.Name.ToUpper().Contains(s)||x.HcDecision.Name.ToUpper().Contains(s)||
       x.AdmissionType.Name.ToUpper().Contains(s)||
       (x.MilitaryClass!=null&&x.MilitaryClass.Name.ToUpper().Contains(s)));
   }
   var count=await q.CountAsync(ct);
   var rows=await q.OrderBy(x=>x.SYYDiagnosis.Code).ThenBy(x=>x.HcDecision.Code).ThenBy(x=>x.Id)
    .Skip((r.PageNumber-1)*r.PageSize).Take(r.PageSize)
    .Select(x=>new SYYDiagnosisDecisionResponse(
      x.Id,x.SYYDiagnosisDefId,x.SYYDiagnosis.Code,x.SYYDiagnosis.Name,
      x.HcDecisionDefId,x.HcDecision.Code,x.HcDecision.Name,
      x.HcAdmissionTypeDefId,x.AdmissionType.Name,
      x.MilitaryClassDefId,x.MilitaryClass!=null?x.MilitaryClass.Name:null,
      x.Amount,x.IsActive,x.Version,x.CreatedDate)).ToListAsync(ct);
   return new PagedResult<SYYDiagnosisDecisionResponse>{Items=rows,TotalCount=count,PageNumber=r.PageNumber,PageSize=r.PageSize};
 }
}
