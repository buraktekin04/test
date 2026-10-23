using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ICD10Diagnostics.Responses;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: adların FK ile EF projeksiyonundan okunması ve xmin sürümü.</summary>
public sealed record ICD10DiagnosticResponse(Guid Id,Guid ICD10DefId,string ICD10Code,string ICD10Name,Guid SYYDiagnosisDefId,string SYYDiagnosisName,Guid HcAdmissionTypeDefId,string HcAdmissionTypeName,int Amount,bool IsActive,uint Version,DateTime CreatedDate)
{

}
