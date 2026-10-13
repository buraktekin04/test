using KLMN.Domain.Common;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Domain.Entities.MilitaryUnit;

/// <summary>Kurum içerisindeki hiyerarşik birlikni temsil eder.</summary>
public sealed class MilitaryUnitDef : BaseEntity
{
    /// <summary>Uygulama içinde benzersiz tanımlama ve referans için kullanılan teknik koddur.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Birliğin kullanıcıya gösterilen adıdır.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Harici sistemlerde kullanılan birlik kodudur.</summary>
    public string? ExternalCode { get; set; }
    /// <summary>Eski sistemdeki kuvvet komutanlığı referansıdır. Ayrı tanım henüz taşınmadığından nullable kimlik olarak saklanır.</summary>
    public Guid? ForcesCommandDef { get; set; }
    /// <summary>Görev yeri olarak seçilebileceği UTC başlangıç; null ise alt sınır yoktur.</summary>
    public DateTime? ValidFrom { get; set; }
    /// <summary>Görev yeri olarak seçilebileceği UTC bitiş (hariç); null ise üst sınır yoktur.</summary>
    public DateTime? ValidTo { get; set; }
    /// <summary>Üst birliğin kimliğidir; kök birimlerde null olabilir.</summary>
    public Guid? ConnectedTopUnity { get; set; }
    /// <summary>Hiyerarşideki üst birime ait navigation özelliğidir.</summary>
    public MilitaryUnitDef? ConnectedTopUnit { get; set; }
    /// <summary>Mevcut birliğe doğrudan bağlı alt birimlerin koleksiyonudur.</summary>
    public ICollection<MilitaryUnitDef> Children { get; set; } = new List<MilitaryUnitDef>();
    /// <summary>Birliğe bağlı kullanıcıların navigation koleksiyonudur.</summary>
    public ICollection<User> Users { get; set; } = new List<User>();
}
