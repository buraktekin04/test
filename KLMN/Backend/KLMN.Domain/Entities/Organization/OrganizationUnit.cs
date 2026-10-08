using KLMN.Domain.Common;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Domain.Entities.Organization;

/// <summary>Kurum içerisindeki hiyerarşik organizasyon birimini temsil eder.</summary>
public sealed class OrganizationUnit : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentOrganizationUnitId { get; set; }
    public OrganizationUnit? ParentOrganizationUnit { get; set; }
    public ICollection<OrganizationUnit> Children { get; set; } = new List<OrganizationUnit>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
