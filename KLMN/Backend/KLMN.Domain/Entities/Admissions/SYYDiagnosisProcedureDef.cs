using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY Gereken Tetkikler — SYY tanısı ile mevcut ProcedureDef arasındaki yaş/cinsiyet koşullu bağlantı.</summary>
public sealed class SYYDiagnosisProcedureDef:BaseEntity
{
    public Guid SYYDiagnosisDefId {get;set;}
    public SYYDiagnosisDef Diagnosis {get;set;}=null!;
    public Guid ProcedureDefId {get;set;}
    public ProcedureDef Procedure {get;set;}=null!;
    public int StartingAge {get;set;}
    public int EndingAge {get;set;}
    /// <summary>Eski sistemin sayısal cinsiyet kodu; anlamı teyit edilene kadar değiştirilmez.</summary>
    public int Gender {get;set;}
}
