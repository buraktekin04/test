using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Sevk durumlarının DB'de saklanan tanımları; şu an için yönetim ekranı yoktur.</summary>
public sealed class DispatchStatusDef : BaseEntity
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
