using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisDecisions.Exceptions;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisionById;
/// <summary>Formun güncel alanlarını ve xmin sürümünü getirir.</summary>
public sealed class GetSYYDiagnosisDecisionByIdQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetSYYDiagnosisDecisionByIdQuery,SYYDiagnosisDecisionResponse>
{
 public async Task<SYYDiagnosisDecisionResponse> Handle(GetSYYDiagnosisDecisionByIdQuery r,CancellationToken ct)
 {
   return await db.SYYDiagnosisDecisions.IgnoreQueryFilters().AsNoTracking()
    .Where(x=>x.Id==r.Id&&!x.IsDeleted)
    .Select(x=>new SYYDiagnosisDecisionResponse(
      x.Id,x.SYYDiagnosisDefId,x.SYYDiagnosis.Code,x.SYYDiagnosis.Name,
      x.HcDecisionDefId,x.HcDecision.Code,x.HcDecision.Name,
      x.HcAdmissionTypeDefId,x.AdmissionType.Name,
      x.MilitaryClassDefId,x.MilitaryClass!=null?x.MilitaryClass.Name:null,
      x.Amount,x.IsActive,x.Version,x.CreatedDate))
    .SingleOrDefaultAsync(ct)??throw new SYYDiagnosisDecisionNotFoundException();
 }
}
