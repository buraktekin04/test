using KLMN.Domain.Common;
using KLMN.Domain.Entities.Geography;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>
/// Askerî okul tanımıdır. UTC ValidFrom/ValidTo ve BaseEntity ortak
/// audit, soft-delete ve xmin özelliklerini kalıtımla kullanır.
/// </summary>
public sealed class MilitarySchoolDef : EffectiveDatedEntity
{
    /// <summary>Silinmemiş okullar arasında benzersiz teknik kod.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Askerî okulun kullanıcıya görünen adı.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Eski MilitarySchoolDef.CityDef nullable GUID alanının FK karşılığı.</summary>
    public Guid? CityId { get; set; }

    /// <summary>Okulun bağlı olduğu şehir; bağlantı isteğe bağlıdır.</summary>
    public CityDef? City { get; set; }

    /// <summary>Eski MilitarySchoolDef.ForcesCommandDef nullable GUID alanının FK karşılığı.</summary>
    public Guid? ForcesCommandDefId { get; set; }

    /// <summary>Okulun bağlı olduğu kuvvet komutanlığı.</summary>
    public ForcesCommandDef? ForcesCommand { get; set; }
}
