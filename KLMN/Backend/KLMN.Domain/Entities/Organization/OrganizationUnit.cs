using KLMN.Domain.Common;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Domain.Entities.Organization;

/// <summary>Kurum içerisindeki hiyerarşik organizasyon birimini temsil eder.</summary>
public sealed class OrganizationUnit : BaseEntity
{
    /// <summary>Uygulama içinde benzersiz tanımlama ve referans için kullanılan teknik koddur.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Rolün, iznin veya organizasyon biriminin kullanıcıya gösterilen adıdır.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Üst organizasyon biriminin kimliğidir; kök birimlerde null olabilir.</summary>
    public Guid? ParentOrganizationUnitId { get; set; }
    /// <summary>Hiyerarşideki üst birime ait navigation özelliğidir.</summary>
    public OrganizationUnit? ParentOrganizationUnit { get; set; }
    /// <summary>Mevcut organizasyon birimine doğrudan bağlı alt birimlerin koleksiyonudur.</summary>
    public ICollection<OrganizationUnit> Children { get; set; } = new List<OrganizationUnit>();
    /// <summary>Organizasyon birimine bağlı kullanıcıların navigation koleksiyonudur.</summary>
    public ICollection<User> Users { get; set; } = new List<User>();
}
