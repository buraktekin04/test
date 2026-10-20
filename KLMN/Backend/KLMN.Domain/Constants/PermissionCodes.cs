namespace KLMN.Domain.Constants;

/// <summary>
/// Uygulama genelinde kullanılan permission kodlarını merkezi ve
/// type-safe şekilde tanımlar.
/// </summary>
public static class PermissionCodes
{
    /// <summary>Kullanıcı yönetimi permission kodlarını içerir.</summary>
    public static class Users
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Users.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Users.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "Users.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "Users.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "Users.Delete";
        /// <summary>Pasif kaydı tekrar aktif duruma getirme yetkisinin teknik kodudur.</summary>
        public const string Activate = "Users.Activate";
        /// <summary>Kullanıcıya rol atama ve rolünü değiştirme yetkisinin kodudur.</summary>
        public const string AssignRole = "Users.AssignRole";
        /// <summary>Rol veya kullanıcı bazlı yetki yönetimi yapabilme kodudur.</summary>
        public const string ManagePermissions = "Users.ManagePermissions";

        /// <summary>Kullanıcı PDF dokümanlarını listeleme ve indirme yetkisidir.</summary>
        public const string ViewDocuments = "Users.ViewDocuments";

        /// <summary>Kullanıcı PDF dokümanlarını yükleme ve silme yetkisidir.</summary>
        public const string ManageDocuments = "Users.ManageDocuments";
    }

    /// <summary>Rol yönetimi permission kodlarını içerir.</summary>
    public static class Roles
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Roles.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Roles.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "Roles.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "Roles.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "Roles.Delete";
        /// <summary>Rol veya kullanıcı bazlı yetki yönetimi yapabilme kodudur.</summary>
        public const string ManagePermissions = "Roles.ManagePermissions";
    }

    /// <summary>Birlik yönetimi permission kodlarını içerir.</summary>
    public static class ForcesCommands
    {
        public const string View = "ForcesCommands.View";
        public const string Query = "ForcesCommands.Query";
        public const string Create = "ForcesCommands.Create";
        public const string Update = "ForcesCommands.Update";
        public const string Delete = "ForcesCommands.Delete";
    }

    /// <summary>Alım numarası tanımlarına ait eylem izinleri.</summary>
    public static class Countries
    {
        public const string View = "Countries.View";
        public const string Query = "Countries.Query";
        public const string Create = "Countries.Create";
        public const string Update = "Countries.Update";
        public const string Delete = "Countries.Delete";
    }
    public static class Cities
    {
        public const string View = "Cities.View";
        public const string Query = "Cities.Query";
        public const string Create = "Cities.Create";
        public const string Update = "Cities.Update";
        public const string Delete = "Cities.Delete";
    }

    /// <summary>İlçe Tanımları modülünün rol ve kullanıcı izin kodları.</summary>
    public static class Towns
    {
        public const string View = "Towns.View";
        public const string Query = "Towns.Query";
        public const string Create = "Towns.Create";
        public const string Update = "Towns.Update";
        public const string Delete = "Towns.Delete";
    }

    /// <summary>Askerî Okul Tanımları için eylem bazlı permission kodları.</summary>
    /// <summary>Askerî Sınıf Tanımları için eylem bazlı yetki kodları.</summary>
    /// <summary>Kuvvete bağlı askerî sınıf grubu tanım modülü.</summary>
    public static class MilitaryClassGroups
    {
        public const string View = "MilitaryClassGroups.View";
        public const string Query = "MilitaryClassGroups.Query";
        public const string Create = "MilitaryClassGroups.Create";
        public const string Update = "MilitaryClassGroups.Update";
        public const string Delete = "MilitaryClassGroups.Delete";
    }

    /// <summary>Kuvvet-sınıf-grup eşleştirmesi modülü.</summary>
    public static class MilitaryClassGroupRelations
    {
        public const string View = "MilitaryClassGroupRelations.View";
        public const string Query = "MilitaryClassGroupRelations.Query";
        public const string Create = "MilitaryClassGroupRelations.Create";
        public const string Update = "MilitaryClassGroupRelations.Update";
        public const string Delete = "MilitaryClassGroupRelations.Delete";
    }

    public static class MilitaryClasses
    {
        public const string View = "MilitaryClasses.View";
        public const string Query = "MilitaryClasses.Query";
        public const string Create = "MilitaryClasses.Create";
        public const string Update = "MilitaryClasses.Update";
        public const string Delete = "MilitaryClasses.Delete";
    }

    /// <summary>Askerlik şubesi tanımlarının işlem yetkileri.</summary>
    /// <summary>Başvuru Grubu Tanımları izin kodları.</summary>
    /// <summary>Bağlı Kurul Karar Tanımları eylem bazlı yetkileri.</summary>
    /// <summary>Branş Tanımları yetki kodları.</summary>
    /// <summary>Rapor Türü modülü için işlem bazlı yetkiler.</summary>
    public static class ReportTypes
    {
        public const string View = "ReportTypes.View";
        public const string Query = "ReportTypes.Query";
        public const string Create = "ReportTypes.Create";
        public const string Update = "ReportTypes.Update";
        public const string Delete = "ReportTypes.Delete";
    }

    /// <summary>Rütbe Grubu modülü için işlem bazlı yetkiler.</summary>
    public static class RankGroups
    {
        public const string View = "RankGroups.View";
        public const string Query = "RankGroups.Query";
        public const string Create = "RankGroups.Create";
        public const string Update = "RankGroups.Update";
        public const string Delete = "RankGroups.Delete";
    }

    /// <summary>Rütbe modülü için işlem bazlı yetkiler.</summary>
    public static class Ranks
    {
        public const string View = "Ranks.View";
        public const string Query = "Ranks.Query";
        public const string Create = "Ranks.Create";
        public const string Update = "Ranks.Update";
        public const string Delete = "Ranks.Delete";
    }

    public static class Branches
    {
        public const string View = "Branches.View";
        public const string Query = "Branches.Query";
        public const string Create = "Branches.Create";
        public const string Update = "Branches.Update";
        public const string Delete = "Branches.Delete";
    }

    public static class ConnectedDecisions
    {
        public const string View = "ConnectedDecisions.View";
        public const string Query = "ConnectedDecisions.Query";
        public const string Create = "ConnectedDecisions.Create";
        public const string Update = "ConnectedDecisions.Update";
        public const string Delete = "ConnectedDecisions.Delete";
    }

    public static class AdmissionTypes
    {
        public const string View = "AdmissionTypes.View";
        public const string Query = "AdmissionTypes.Query";
        public const string Create = "AdmissionTypes.Create";
        public const string Update = "AdmissionTypes.Update";
        public const string Delete = "AdmissionTypes.Delete";
    }

    public static class MilitaryOffices
    {
        public const string View = "MilitaryOffices.View";
        public const string Query = "MilitaryOffices.Query";
        public const string Create = "MilitaryOffices.Create";
        public const string Update = "MilitaryOffices.Update";
        public const string Delete = "MilitaryOffices.Delete";
    }

    public static class MilitarySchools
    {
        public const string View = "MilitarySchools.View";
        public const string Query = "MilitarySchools.Query";
        public const string Create = "MilitarySchools.Create";
        public const string Update = "MilitarySchools.Update";
        public const string Delete = "MilitarySchools.Delete";
    }

    public static class PurchaseNumbers
    {
        public const string View = "PurchaseNumbers.View";
        public const string Query = "PurchaseNumbers.Query";
        public const string Create = "PurchaseNumbers.Create";
        public const string Update = "PurchaseNumbers.Update";
        public const string Delete = "PurchaseNumbers.Delete";
    }

    public static class MilitaryUnits
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "MilitaryUnits.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "MilitaryUnits.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "MilitaryUnits.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "MilitaryUnits.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "MilitaryUnits.Delete";
    }

    /// <summary>Raporlama permission kodlarını içerir.</summary>
    public static class Reports
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Reports.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Reports.Query";
        /// <summary>Kayıtları dışa aktarabilme yetkisinin kodudur.</summary>
        public const string Export = "Reports.Export";
        /// <summary>Kayıtların yazıcıya aktarılabilmesi için gereken yetki kodudur.</summary>
        public const string Print = "Reports.Print";
    }

    /// <summary>Tanımlama geçmişini görüntüleme yetkisidir.</summary>
    public static class AuditLogs
    {
        public const string View = "AuditLogs.View";
    }

    /// <summary>İnceleme ve soruşturma permission kodlarını içerir.</summary>
    public static class Investigations
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Investigations.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Investigations.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "Investigations.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "Investigations.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "Investigations.Delete";
        /// <summary>İnceleme veya soruşturma kaydını onaylama yetkisinin kodudur.</summary>
        public const string Approve = "Investigations.Approve";
        /// <summary>İnceleme veya soruşturma kaydını reddetme yetkisinin kodudur.</summary>
        public const string Reject = "Investigations.Reject";
        /// <summary>İnceleme veya soruşturma sürecini kapatma yetkisinin kodudur.</summary>
        public const string Close = "Investigations.Close";
        /// <summary>Kayıtları dışa aktarabilme yetkisinin kodudur.</summary>
        public const string Export = "Investigations.Export";
        /// <summary>Kayıtların yazıcıya aktarılabilmesi için gereken yetki kodudur.</summary>
        public const string Print = "Investigations.Print";
    }
}
