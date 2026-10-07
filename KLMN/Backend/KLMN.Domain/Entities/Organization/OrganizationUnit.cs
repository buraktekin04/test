using KLMN.Domain.Common;
using KLMN.Domain.Entities.Identity;

namespace KLMN.Domain.Entities.Organization;

/// <summary>
/// Kurum içerisindeki organizasyon/birim hiyerarşisini temsil eder.
/// </summary>
public sealed class OrganizationUnit : BaseEntity
{
    /// <summary>
    /// Birimin benzersiz iş kodudur.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Birimin adıdır.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Üst birimin kimliğidir.
    /// </summary>
    public Guid? ParentOrganizationUnitId { get; set; }

    /// <summary>
    /// Üst birim navigation alanıdır.
    /// </summary>
    public OrganizationUnit? ParentOrganizationUnit { get; set; }

    /// <summary>
    /// Alt organizasyon birimlerini içerir.
    /// </summary>
    public ICollection<OrganizationUnit> Children { get; set; } =
        new HashSet<OrganizationUnit>();

    /// <summary>
    /// Birime bağlı kullanıcıları içerir.
    /// </summary>
    public ICollection<User> Users { get; set; } =
        new HashSet<User>();
}
