using KLMN.Domain.Common;

namespace KLMN.Domain.Entities.Identity;

/// <summary>Belirli bir ekran, işlem veya iş aksiyonuna erişim hakkını temsil eder.</summary>
public sealed class Permission : BaseEntity
{
    /// <summary>Rolün, iznin veya organizasyon biriminin kullanıcıya gösterilen adıdır.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Uygulama içinde benzersiz tanımlama ve referans için kullanılan teknik koddur.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Yetkinin ait olduğu fonksiyonel modülün adıdır.</summary>
    public string Module { get; set; } = string.Empty;
    /// <summary>Kaydın işlevini açıklayan isteğe bağlı metindir.</summary>
    public string? Description { get; set; }
    /// <summary>Yetki yönetimi arayüzünde kullanılacak sıralama değeridir.</summary>
    public int SortOrder { get; set; }
    /// <summary>Rolün izin kümesini oluşturan role-permission bağlantılarıdır.</summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    /// <summary>Kullanıcı için rol yetkisinden bağımsız tanımlanmış izin/red override kayıtlarıdır.</summary>
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
