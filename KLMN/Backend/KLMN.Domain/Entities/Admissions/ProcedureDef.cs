using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>Tetkik Tanımları. Eski ProcedureDef alan adları ve tipleri korunur; audit ve aktiflik BaseEntity'dedir.</summary>
public sealed class ProcedureDef : BaseEntity
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
    /// <summary>Eski modelin isteğe bağlı başlangıç tarihi (UTC).</summary>
    public DateTime? StartDate { get; set; }
    /// <summary>Eski modelin isteğe bağlı bitiş tarihi (UTC).</summary>
    public DateTime? EndDate { get; set; }
    /// <summary>Eski enumun sayısal kodu; ProcedureTypeDefs.Code alanını referanslar.</summary>
    public int ProcedureType { get; set; }
    /// <summary>Dinamik tetkik türü kaydı.</summary>
    public ProcedureTypeDef TypeDefinition { get; set; } = null!;
    /// <summary>Muayene sebebi eşleştirmeleri.</summary>
    public ICollection<ReasonForExamProcedureDef> ExaminationReasons { get; set; } = new List<ReasonForExamProcedureDef>();
}
