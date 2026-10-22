using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY Onay Gerektiren Maddeler: bir tanıya bağlı onay kuralı ve alt açıklamaları.</summary>
public sealed class SYYDiagnosisRequired:BaseEntity
{
 public Guid SYYDiagnosisDefId {get;set;}
 public SYYDiagnosisDef Diagnosis {get;set;}=null!;
 /// <summary>Bütün maddeler zorunlu (null = tanımsız).</summary>
 public bool? MustAll {get;set;}
 /// <summary>En az bir madde zorunlu (null = tanımsız).</summary>
 public bool? MustOneOf {get;set;}
 public ICollection<SYYDiagnosisRequiredDetail> Details {get;set;}=new List<SYYDiagnosisRequiredDetail>();
}
