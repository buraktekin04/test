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

    /// <summary>Organizasyon yönetimi permission kodlarını içerir.</summary>
    public static class Organizations
    {
        /// <summary>Modül kayıtlarını görüntüleyebilme yetkisinin teknik kodudur.</summary>
        public const string View = "Organizations.View";
        /// <summary>Modül kayıtlarında sorgu ve arama yapabilme yetkisinin teknik kodudur.</summary>
        public const string Query = "Organizations.Query";
        /// <summary>Yeni kayıt oluşturabilme yetkisinin teknik kodudur.</summary>
        public const string Create = "Organizations.Create";
        /// <summary>Mevcut kaydı düzenleyebilme yetkisinin teknik kodudur.</summary>
        public const string Update = "Organizations.Update";
        /// <summary>Kaydı silme yetkisinin teknik kodudur.</summary>
        public const string Delete = "Organizations.Delete";
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
