using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY Tanı Listesi (M/D/F) Ekranı. SYY tanı türü, bağlı tanı ve dilimle birlikte yönetilir.</summary>
public sealed class SYYDiagnosisDef:EffectiveDatedEntity
{
    public string Code {get;set;}=string.Empty;
    public string Name {get;set;}=string.Empty;
    /// <summary>Tanı Türü: SYYDiagnosisTypeDefs.Code FK (int).</summary>
    public int SYYDiagnosisType {get;set;}
    public SYYDiagnosisTypeDef DiagnosisTypeDef {get;set;}=null!;
    /// <summary>Kendi tablosundaki bağlı kayıt; id üzerinden self-FK.</summary>
    public Guid? AttachedRecNo {get;set;}
    public SYYDiagnosisDef? AttachedDiagnosis {get;set;}
    public ICollection<SYYDiagnosisDef> AttachedChildren {get;set;}=new List<SYYDiagnosisDef>();
    /// <summary>Seçilebilir: null, bilinmiyor anlamını korur.</summary>
    public bool? Eligible {get;set;}
    /// <summary>Bağlı Dilim: SYYDiagnosisSliceDefs.Code FK (int nullable).</summary>
    public int? Slice {get;set;}
    public SYYDiagnosisSliceDef? SliceDef {get;set;}
    /// <summary>SYY sınıflandırma çizelgesi satırları.</summary>
    public ICollection<SYYClassificationDef> Classifications {get;set;}=new List<SYYClassificationDef>();
    /// <summary>Bu tanıda gereken branşlar.</summary>
    public ICollection<SYYDiagnosisBranchDef> RequiredBranches {get;set;}=new List<SYYDiagnosisBranchDef>();
    /// <summary>Bu tanıda gereken tetkikler.</summary>
    public ICollection<SYYDiagnosisProcedureDef> RequiredProcedures {get;set;}=new List<SYYDiagnosisProcedureDef>();
    /// <summary>Onay gerektiren maddeler / kurallar.</summary>
    public ICollection<SYYDiagnosisRequired> RequiredRules {get;set;}=new List<SYYDiagnosisRequired>();
    /// <summary>ICD10 SYY tanı istatistikleri.</summary>
    public ICollection<ICD10Diagnostic> ICD10Diagnostics {get;set;}=new List<ICD10Diagnostic>();
}
