using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY Gereken Branşlar — SYY tanısı ile mevcut BranchDef arasındaki yaş/cinsiyet koşullu bağlantı.</summary>
public sealed class SYYDiagnosisBranchDef:BaseEntity
{
    public Guid SYYDiagnosisDefId {get;set;}
    public SYYDiagnosisDef Diagnosis {get;set;}=null!;
    public Guid BranchDefId {get;set;}
    public BranchDef Branch {get;set;}=null!;
    public int StartingAge {get;set;}
    public int EndingAge {get;set;}
    /// <summary>Eski sistemin sayısal cinsiyet kodu; anlamı teyit edilene kadar değiştirilmez.</summary>
    public int Gender {get;set;}
}
