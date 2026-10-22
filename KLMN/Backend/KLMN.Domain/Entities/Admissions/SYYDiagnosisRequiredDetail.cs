using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY Onay Gerektiren Maddeler: kurala ait sıralı metin maddeleri, BaseEntity audit.</summary>
public sealed class SYYDiagnosisRequiredDetail:BaseEntity
{
 public Guid SYYDiagnosisRequiredId {get;set;}
 public SYYDiagnosisRequired Required {get;set;}=null!;
 public string Description {get;set;}=string.Empty;
 public int SortOrder {get;set;}
}
