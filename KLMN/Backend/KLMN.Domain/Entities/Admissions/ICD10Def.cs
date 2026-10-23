using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>ICD10 Tanımları ekranı. Audit, durum, soft-delete ve xmin BaseEntity üzerinden gelir.</summary>
public sealed class ICD10Def:BaseEntity
{
 /// <summary>ICD-10 kodu: nokta ve harfler korunur, büyük harfle saklanır.</summary>
 public string Code {get;set;}=string.Empty;
 public string Name {get;set;}=string.Empty;
 public ICollection<ICD10Diagnostic> Diagnostics {get;set;}=new List<ICD10Diagnostic>();
}
