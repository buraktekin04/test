using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Geography;

/// <summary>Eski CityDef; ValidFrom/ValidTo ayrı açılmaz, EffectiveDatedEntity'den gelir.</summary>
public sealed class CityDef : EffectiveDatedEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ExternalCode { get; set; }
    /// <summary>Eski CityDef.Country alanı; ülke seçimi isteğe bağlıdır.</summary>
    public Guid? CountryId { get; set; }
    public CountryDef? Country { get; set; }
    public int? MernisCode { get; set; }

    /// <summary>Şehre bağlı ilçeler; ilişkiyi bozmamak için silmede FK Restrict uygulanır.</summary>
    public ICollection<TownDef> Towns { get; set; } = new List<TownDef>();

    /// <summary>Şehre bağlı askerî okullar; FK ile referans bütünlüğü korunur.</summary>
    public ICollection<KLMN.Domain.Entities.MilitaryUnit.MilitarySchoolDef> MilitarySchools { get; set; }
        = new List<KLMN.Domain.Entities.MilitaryUnit.MilitarySchoolDef>();

    /// <summary>Şehirle ilişkili askerlik şubeleri; silmede FK Restrict uygulanır.</summary>
    public ICollection<KLMN.Domain.Entities.MilitaryUnit.MilitaryOfficeDef> MilitaryOffices { get; set; }
        = new List<KLMN.Domain.Entities.MilitaryUnit.MilitaryOfficeDef>();
}
