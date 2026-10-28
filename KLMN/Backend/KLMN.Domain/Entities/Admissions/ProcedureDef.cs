using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Tetkik Tanımları: geçerlilik tarihleri EffectiveDatedEntity, kimlik/audit/aktiflik/xmin BaseEntity üzerinden kalıtımla gelir.</summary>
public sealed class ProcedureDef : EffectiveDatedEntity
{
    /// <summary>Eski sistemdeki ProcedureCode alanı.</summary>
    public string ProcedureCode { get; set; } = string.Empty;
    /// <summary>Tetkik adı.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Numerik Sonuç Gerektirir.</summary>
    public bool RequiresNumericResult { get; set; }
    /// <summary>Referans Aralığı Gerektirir.</summary>
    public bool RequiresReferenceRange { get; set; }
    /// <summary>Sapma Bilgisi Gerektirir.</summary>
    public bool RequiresDeviationInfo { get; set; }
    // ValidFrom ve ValidTo, EffectiveDatedEntity üzerinden miras alınır.
    /// <summary>Eski sistemle API uyumlu kod; FK artık ProcedureTypeDefId üzerindedir.</summary>
    public int ProcedureType { get; set; }
    /// <summary>Dinamik tetkik türü kaydı.</summary>
    public Guid ProcedureTypeDefId { get; set; }
    public ProcedureTypeDef TypeDefinition { get; set; } = null!;
    /// <summary>Muayene sebebi eşleştirmeleri.</summary>
    public ICollection<ReasonForExamProcedureDef> ExaminationReasons { get; set; } = new List<ReasonForExamProcedureDef>();
}
