using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ICD10Diagnostics.Responses;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: adların FK ile EF projeksiyonundan okunması ve xmin sürümü.</summary>
public sealed record ICD10DiagnosticResponse(Guid Id,Guid ICD10DefId,string ICD10Code,string ICD10Name,Guid SYYDiagnosisDefId,string SYYDiagnosisName,Guid HcAdmissionTypeDefId,string HcAdmissionTypeName,int Amount,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<ICD10Diagnostic,ICD10DiagnosticResponse>> Projection=>x=>
 new(x.Id,x.ICD10DefId,x.ICD10.Code,x.ICD10.Name,x.SYYDiagnosisDefId,x.SYYDiagnosis.Code+" — "+x.SYYDiagnosis.Name,x.HcAdmissionTypeDefId,x.AdmissionType.Name,x.Amount,x.IsActive,x.Version,x.CreatedDate);
}
public sealed record ICD10DiagnosticMutationResponse(Guid Id,uint Version);
