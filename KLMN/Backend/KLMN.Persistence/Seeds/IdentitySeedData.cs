using KLMN.Domain.Constants;

namespace KLMN.Persistence.Seeds;

/// <summary>Başlangıç rol ve permission tanımlarını içerir.</summary>
internal static class IdentitySeedData
{
    public static IReadOnlyCollection<RoleSeedItem> Roles { get; } =
    [
        new("Yönetici", SystemRoles.Admin, "Sistem genelinde tam yetkiye sahip yönetici rolüdür.", true),
        new("Standart Kullanıcı", SystemRoles.StandardUser, "Sistemdeki standart kullanıcılar için temel roldür.", true),
        new("İnceleme Görevlisi", "INVESTIGATION_OFFICER", "İnceleme ve soruşturma süreçlerini yürüten kullanıcı rolüdür.", false),
        new("Şube Müdürü", "BRANCH_MANAGER", "Şube seviyesindeki yönetim ve iş akışı süreçlerinde kullanılan roldür.", false)
    ];

    public static IReadOnlyCollection<PermissionSeedItem> Permissions { get; } =
    [
        new("Kullanıcıları Görüntüleme", PermissionCodes.Users.View, "Users", 10),
        new("Kullanıcı Sorgulama", PermissionCodes.Users.Query, "Users", 20),
        new("Kullanıcı Oluşturma", PermissionCodes.Users.Create, "Users", 30),
        new("Kullanıcı Güncelleme", PermissionCodes.Users.Update, "Users", 40),
        new("Kullanıcı Silme", PermissionCodes.Users.Delete, "Users", 50),
        new("Kullanıcı Aktifleştirme", PermissionCodes.Users.Activate, "Users", 60),
        new("Kullanıcıya Rol Atama", PermissionCodes.Users.AssignRole, "Users", 70),
        new("Kullanıcı Yetkilerini Yönetme", PermissionCodes.Users.ManagePermissions, "Users", 80),

        new("Rolleri Görüntüleme", PermissionCodes.Roles.View, "Roles", 10),
        new("Rol Sorgulama", PermissionCodes.Roles.Query, "Roles", 20),
        new("Rol Oluşturma", PermissionCodes.Roles.Create, "Roles", 30),
        new("Rol Güncelleme", PermissionCodes.Roles.Update, "Roles", 40),
        new("Rol Silme", PermissionCodes.Roles.Delete, "Roles", 50),
        new("Rol Yetkilerini Yönetme", PermissionCodes.Roles.ManagePermissions, "Roles", 60),

        new("Organizasyonları Görüntüleme", PermissionCodes.Organizations.View, "Organizations", 10),
        new("Organizasyon Sorgulama", PermissionCodes.Organizations.Query, "Organizations", 20),
        new("Organizasyon Oluşturma", PermissionCodes.Organizations.Create, "Organizations", 30),
        new("Organizasyon Güncelleme", PermissionCodes.Organizations.Update, "Organizations", 40),
        new("Organizasyon Silme", PermissionCodes.Organizations.Delete, "Organizations", 50),

        new("Raporları Görüntüleme", PermissionCodes.Reports.View, "Reports", 10),
        new("Rapor Sorgulama", PermissionCodes.Reports.Query, "Reports", 20),
        new("Rapor Dışa Aktarma", PermissionCodes.Reports.Export, "Reports", 30),
        new("Rapor Yazdırma", PermissionCodes.Reports.Print, "Reports", 40),

        new("İncelemeleri Görüntüleme", PermissionCodes.Investigations.View, "Investigations", 10),
        new("İnceleme Sorgulama", PermissionCodes.Investigations.Query, "Investigations", 20),
        new("İnceleme Oluşturma", PermissionCodes.Investigations.Create, "Investigations", 30),
        new("İnceleme Güncelleme", PermissionCodes.Investigations.Update, "Investigations", 40),
        new("İnceleme Silme", PermissionCodes.Investigations.Delete, "Investigations", 50),
        new("İnceleme Onaylama", PermissionCodes.Investigations.Approve, "Investigations", 60),
        new("İnceleme Reddetme", PermissionCodes.Investigations.Reject, "Investigations", 70),
        new("İnceleme Kapatma", PermissionCodes.Investigations.Close, "Investigations", 80),
        new("İnceleme Dışa Aktarma", PermissionCodes.Investigations.Export, "Investigations", 90),
        new("İnceleme Yazdırma", PermissionCodes.Investigations.Print, "Investigations", 100)
    ];
}

internal sealed record RoleSeedItem(
    string Name,
    string Code,
    string Description,
    bool IsSystemRole);

internal sealed record PermissionSeedItem(
    string Name,
    string Code,
    string Module,
    int SortOrder);

