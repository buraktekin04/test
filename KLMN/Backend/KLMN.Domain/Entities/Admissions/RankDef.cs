using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Admissions;

/// <summary>Rütbe tanımı. Ortak audit, aktiflik ve tarih alanları miras alınır.</summary>
public sealed class RankDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş kayıtlar arasında benzersiz rütbe kodu.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Rütbenin adı.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Rütbenin kısa adı.</summary>
    public string? ShortName { get; set; }
    /// <summary>Eski sistemdeki Type (int); anlamı doğrulanana kadar sayısal tutulur.</summary>
    public int Type { get; set; }
    /// <summary>Eski RankGroup alanının nullable RankGroupDef yabancı anahtarı.</summary>
    public Guid? RankGroupId { get; set; }
    public RankGroupDef? RankGroup { get; set; }
}
