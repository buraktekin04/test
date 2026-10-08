using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Kullanıcılara atanabilen dinamik yetkilendirme rolünü temsil eder.</summary>
public sealed class Role : BaseEntity
{
    /// <summary>Rolün, iznin veya organizasyon biriminin kullanıcıya gösterilen adıdır.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Uygulama içinde benzersiz tanımlama ve referans için kullanılan teknik koddur.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Kaydın işlevini açıklayan isteğe bağlı metindir.</summary>
    public string? Description { get; set; }
    /// <summary>Rolün sistem tarafından korunan özel bir rol olduğunu gösterir.</summary>
    public bool IsSystemRole { get; set; }
    /// <summary>Kullanıcının birden fazla rolle olan bağlantılarını tutan navigation koleksiyonudur.</summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    /// <summary>Rolün izin kümesini oluşturan role-permission bağlantılarıdır.</summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
