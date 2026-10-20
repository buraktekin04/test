using KLMN.Domain.Constants;

namespace KLMN.Persistence.Seeds;

/// <summary>Başlangıç rol ve permission tanımlarını içerir.</summary>
internal static class IdentitySeedData
{
    /// <summary>
    /// Tanımlanan dinamik ve sistem rollerinin EF Core DbSet koleksiyonudur.
    /// </summary>
    public static IReadOnlyCollection<RoleSeedItem> Roles { get; } =
    [
        new("Yönetici", SystemRoles.Admin, "Sistem genelinde tam yetkiye sahip yönetici rolüdür.", true),
        new("Standart Kullanıcı", SystemRoles.StandardUser, "Sistemdeki standart kullanıcılar için temel roldür.", true),
        new("İnceleme Görevlisi", "INVESTIGATION_OFFICER", "İnceleme ve soruşturma süreçlerini yürüten kullanıcı rolüdür.", false),
        new("Şube Müdürü", "BRANCH_MANAGER", "Şube seviyesindeki yönetim ve iş akışı süreçlerinde kullanılan roldür.", false)
    ];

    /// <summary>
    /// Uygulamanın başlangıçta tanımlanan permission seed kayıtlarını içerir.
    /// </summary>
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
        new("Kullanıcı PDF Dokümanlarını Görüntüleme", PermissionCodes.Users.ViewDocuments, "Users", 90),
        new("Kullanıcı PDF Dokümanlarını Yönetme", PermissionCodes.Users.ManageDocuments, "Users", 100),

        new("Rolleri Görüntüleme", PermissionCodes.Roles.View, "Roles", 10),
        new("Rol Sorgulama", PermissionCodes.Roles.Query, "Roles", 20),
        new("Rol Oluşturma", PermissionCodes.Roles.Create, "Roles", 30),
        new("Rol Güncelleme", PermissionCodes.Roles.Update, "Roles", 40),
        new("Rol Silme", PermissionCodes.Roles.Delete, "Roles", 50),
        new("Rol Yetkilerini Yönetme", PermissionCodes.Roles.ManagePermissions, "Roles", 60),

        new("Sınıf Gruplarını Görüntüleme", PermissionCodes.MilitaryClassGroups.View, "MilitaryClassGroups", 10),
        new("Sınıf Grubu Sorgulama", PermissionCodes.MilitaryClassGroups.Query, "MilitaryClassGroups", 20),
        new("Sınıf Grubu Oluşturma", PermissionCodes.MilitaryClassGroups.Create, "MilitaryClassGroups", 30),
        new("Sınıf Grubu Güncelleme", PermissionCodes.MilitaryClassGroups.Update, "MilitaryClassGroups", 40),
        new("Sınıf Grubu Silme", PermissionCodes.MilitaryClassGroups.Delete, "MilitaryClassGroups", 50),
        new("Sınıf–Grup Eşleştirmelerini Görüntüleme", PermissionCodes.MilitaryClassGroupRelations.View, "MilitaryClassGroupRelations", 10),
        new("Sınıf–Grup Eşleştirme Sorgulama", PermissionCodes.MilitaryClassGroupRelations.Query, "MilitaryClassGroupRelations", 20),
        new("Sınıf–Grup Eşleştirme Oluşturma", PermissionCodes.MilitaryClassGroupRelations.Create, "MilitaryClassGroupRelations", 30),
        new("Sınıf–Grup Eşleştirme Güncelleme", PermissionCodes.MilitaryClassGroupRelations.Update, "MilitaryClassGroupRelations", 40),
        new("Sınıf–Grup Eşleştirme Silme", PermissionCodes.MilitaryClassGroupRelations.Delete, "MilitaryClassGroupRelations", 50),
        new("Askerî Sınıfları Görüntüleme", PermissionCodes.MilitaryClasses.View, "MilitaryClasses", 10),
        new("Askerî Sınıf Sorgulama", PermissionCodes.MilitaryClasses.Query, "MilitaryClasses", 20),
        new("Askerî Sınıf Oluşturma", PermissionCodes.MilitaryClasses.Create, "MilitaryClasses", 30),
        new("Askerî Sınıf Güncelleme", PermissionCodes.MilitaryClasses.Update, "MilitaryClasses", 40),
        new("Askerî Sınıf Silme", PermissionCodes.MilitaryClasses.Delete, "MilitaryClasses", 50),
        new("Rapor Türü Görüntüleme", PermissionCodes.ReportTypes.View, "ReportTypes", 10),
        new("Rapor Türü Sorgulama", PermissionCodes.ReportTypes.Query, "ReportTypes", 20),
        new("Rapor Türü Oluşturma", PermissionCodes.ReportTypes.Create, "ReportTypes", 30),
        new("Rapor Türü Güncelleme", PermissionCodes.ReportTypes.Update, "ReportTypes", 40),
        new("Rapor Türü Silme", PermissionCodes.ReportTypes.Delete, "ReportTypes", 50),
        new("Rütbe Grubu Görüntüleme", PermissionCodes.RankGroups.View, "RankGroups", 10),
        new("Rütbe Grubu Sorgulama", PermissionCodes.RankGroups.Query, "RankGroups", 20),
        new("Rütbe Grubu Oluşturma", PermissionCodes.RankGroups.Create, "RankGroups", 30),
        new("Rütbe Grubu Güncelleme", PermissionCodes.RankGroups.Update, "RankGroups", 40),
        new("Rütbe Grubu Silme", PermissionCodes.RankGroups.Delete, "RankGroups", 50),
        new("Rütbe Görüntüleme", PermissionCodes.Ranks.View, "Ranks", 10),
        new("Rütbe Sorgulama", PermissionCodes.Ranks.Query, "Ranks", 20),
        new("Rütbe Oluşturma", PermissionCodes.Ranks.Create, "Ranks", 30),
        new("Rütbe Güncelleme", PermissionCodes.Ranks.Update, "Ranks", 40),
        new("Rütbe Silme", PermissionCodes.Ranks.Delete, "Ranks", 50),
        new("Branşları Görüntüleme", PermissionCodes.Branches.View, "Branches", 10),
        new("Branş Sorgulama", PermissionCodes.Branches.Query, "Branches", 20),
        new("Branş Oluşturma", PermissionCodes.Branches.Create, "Branches", 30),
        new("Branş Güncelleme", PermissionCodes.Branches.Update, "Branches", 40),
        new("Branş Silme", PermissionCodes.Branches.Delete, "Branches", 50),
        new("Bağlı Kurul Kararlarını Görüntüleme", PermissionCodes.ConnectedDecisions.View, "ConnectedDecisions", 10),
        new("Bağlı Kurul Kararlarını Sorgulama", PermissionCodes.ConnectedDecisions.Query, "ConnectedDecisions", 20),
        new("Bağlı Kurul Kararı Oluşturma", PermissionCodes.ConnectedDecisions.Create, "ConnectedDecisions", 30),
        new("Bağlı Kurul Kararı Güncelleme", PermissionCodes.ConnectedDecisions.Update, "ConnectedDecisions", 40),
        new("Bağlı Kurul Kararı Silme", PermissionCodes.ConnectedDecisions.Delete, "ConnectedDecisions", 50),
        new("Başvuru Gruplarını Görüntüleme", PermissionCodes.AdmissionTypes.View, "AdmissionTypes", 10),
        new("Başvuru Grubu Sorgulama", PermissionCodes.AdmissionTypes.Query, "AdmissionTypes", 20),
        new("Başvuru Grubu Oluşturma", PermissionCodes.AdmissionTypes.Create, "AdmissionTypes", 30),
        new("Başvuru Grubu Güncelleme", PermissionCodes.AdmissionTypes.Update, "AdmissionTypes", 40),
        new("Başvuru Grubu Silme", PermissionCodes.AdmissionTypes.Delete, "AdmissionTypes", 50),
        new("Askerlik Şubelerini Görüntüleme", PermissionCodes.MilitaryOffices.View, "MilitaryOffices", 10),
        new("Askerlik Şubesi Sorgulama", PermissionCodes.MilitaryOffices.Query, "MilitaryOffices", 20),
        new("Askerlik Şubesi Oluşturma", PermissionCodes.MilitaryOffices.Create, "MilitaryOffices", 30),
        new("Askerlik Şubesi Güncelleme", PermissionCodes.MilitaryOffices.Update, "MilitaryOffices", 40),
        new("Askerlik Şubesi Silme", PermissionCodes.MilitaryOffices.Delete, "MilitaryOffices", 50),
        new("Askerî Okulları Görüntüleme", PermissionCodes.MilitarySchools.View, "MilitarySchools", 10),
        new("Askerî Okul Sorgulama", PermissionCodes.MilitarySchools.Query, "MilitarySchools", 20),
        new("Askerî Okul Oluşturma", PermissionCodes.MilitarySchools.Create, "MilitarySchools", 30),
        new("Askerî Okul Güncelleme", PermissionCodes.MilitarySchools.Update, "MilitarySchools", 40),
        new("Askerî Okul Silme", PermissionCodes.MilitarySchools.Delete, "MilitarySchools", 50),
        new("Kuvvet Komutanlıklarını Görüntüleme", PermissionCodes.ForcesCommands.View, "ForcesCommands", 10),
        new("Kuvvet Komutanlığı Sorgulama", PermissionCodes.ForcesCommands.Query, "ForcesCommands", 20),
        new("Kuvvet Komutanlığı Oluşturma", PermissionCodes.ForcesCommands.Create, "ForcesCommands", 30),
        new("Kuvvet Komutanlığı Güncelleme", PermissionCodes.ForcesCommands.Update, "ForcesCommands", 40),
        new("Kuvvet Komutanlığı Silme", PermissionCodes.ForcesCommands.Delete, "ForcesCommands", 50),
        new("Birlikleri Görüntüleme", PermissionCodes.MilitaryUnits.View, "MilitaryUnits", 10),
        new("Birlik Sorgulama", PermissionCodes.MilitaryUnits.Query, "MilitaryUnits", 20),
        new("Birlik Oluşturma", PermissionCodes.MilitaryUnits.Create, "MilitaryUnits", 30),
        new("Birlik Güncelleme", PermissionCodes.MilitaryUnits.Update, "MilitaryUnits", 40),
        new("Birlik Silme", PermissionCodes.MilitaryUnits.Delete, "MilitaryUnits", 50),

        new("Ülkeleri Görüntüleme", PermissionCodes.Countries.View, "Countries", 10),
        new("Ülke Sorgulama", PermissionCodes.Countries.Query, "Countries", 20),
        new("Ülke Oluşturma", PermissionCodes.Countries.Create, "Countries", 30),
        new("Ülke Güncelleme", PermissionCodes.Countries.Update, "Countries", 40),
        new("Ülke Silme", PermissionCodes.Countries.Delete, "Countries", 50),
        new("İlçeleri Görüntüleme", PermissionCodes.Towns.View, "Towns", 10),
        new("İlçe Sorgulama", PermissionCodes.Towns.Query, "Towns", 20),
        new("İlçe Oluşturma", PermissionCodes.Towns.Create, "Towns", 30),
        new("İlçe Güncelleme", PermissionCodes.Towns.Update, "Towns", 40),
        new("İlçe Silme", PermissionCodes.Towns.Delete, "Towns", 50),
        new("Şehirleri Görüntüleme", PermissionCodes.Cities.View, "Cities", 10),
        new("Şehir Sorgulama", PermissionCodes.Cities.Query, "Cities", 20),
        new("Şehir Oluşturma", PermissionCodes.Cities.Create, "Cities", 30),
        new("Şehir Güncelleme", PermissionCodes.Cities.Update, "Cities", 40),
        new("Şehir Silme", PermissionCodes.Cities.Delete, "Cities", 50),

        new("Alım Numaralarını Görüntüleme", PermissionCodes.PurchaseNumbers.View, "PurchaseNumbers", 10),
        new("Alım Numarası Sorgulama", PermissionCodes.PurchaseNumbers.Query, "PurchaseNumbers", 20),
        new("Alım Numarası Oluşturma", PermissionCodes.PurchaseNumbers.Create, "PurchaseNumbers", 30),
        new("Alım Numarası Güncelleme", PermissionCodes.PurchaseNumbers.Update, "PurchaseNumbers", 40),
        new("Alım Numarası Silme", PermissionCodes.PurchaseNumbers.Delete, "PurchaseNumbers", 50),

        new("Raporları Görüntüleme", PermissionCodes.Reports.View, "Reports", 10),
        new("Rapor Sorgulama", PermissionCodes.Reports.Query, "Reports", 20),
        new("Rapor Dışa Aktarma", PermissionCodes.Reports.Export, "Reports", 30),
        new("Rapor Yazdırma", PermissionCodes.Reports.Print, "Reports", 40),

        new("Tanımlama İşlem Geçmişini Görüntüleme", PermissionCodes.AuditLogs.View, "AuditLogs", 10),

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
