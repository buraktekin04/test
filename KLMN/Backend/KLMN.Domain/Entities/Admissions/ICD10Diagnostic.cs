using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları ekranı: ICD10 + SYY + başvuru grubu için miktar.</summary>
public sealed class ICD10Diagnostic:BaseEntity
{
 public Guid ICD10DefId {get;set;}
 public ICD10Def ICD10 {get;set;}=null!;
 public Guid SYYDiagnosisDefId {get;set;}
 public SYYDiagnosisDef SYYDiagnosis {get;set;}=null!;
 public Guid HcAdmissionTypeDefId {get;set;}
 public HcAdmissionTypeDef AdmissionType {get;set;}=null!;
 /// <summary>Eski Amount alanı, negatif olamaz.</summary>
 public int Amount {get;set;}
}
