using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>Birliklerin bağlı olduğu kuvvet komutanlığı tanımıdır.</summary>
public sealed class ForcesCommandDef : EffectiveDatedEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<MilitaryUnitDef> MilitaryUnits { get; set; } = new List<MilitaryUnitDef>();

    /// <summary>Komutanlığa bağlı askerî okullar; silme ve pasifleştirme kısıtlarını belirler.</summary>
    public ICollection<MilitarySchoolDef> MilitarySchools { get; set; } = new List<MilitarySchoolDef>();
}
