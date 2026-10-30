using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Eski MSKBS Gender enum değerlerini koruyan veritabanı tanımı.</summary>
public sealed class GenderDef : BaseEntity
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
