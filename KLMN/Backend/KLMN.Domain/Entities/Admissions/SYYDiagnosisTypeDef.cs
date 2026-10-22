using KLMN.Domain.Common;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY Tanı Türü Tanımları: eski integer kodları koruyan dinamik referans kaydı.</summary>
public sealed class SYYDiagnosisTypeDef:BaseEntity
{
    /// <summary>Eski SYY modelindeki sayısal kod.</summary>
    public int Code { get; set; }
    /// <summary>Tanım adı.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Bu koda bağlı SYY tanıları.</summary>
    public ICollection<SYYDiagnosisDef> Diagnoses { get; set; } = new List<SYYDiagnosisDef>();
}
