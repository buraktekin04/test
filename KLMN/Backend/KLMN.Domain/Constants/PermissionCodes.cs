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
        public const string View = "Users.View";
        public const string Query = "Users.Query";
        public const string Create = "Users.Create";
        public const string Update = "Users.Update";
        public const string Delete = "Users.Delete";
        public const string Activate = "Users.Activate";
        public const string AssignRole = "Users.AssignRole";
        public const string ManagePermissions = "Users.ManagePermissions";
    }

    /// <summary>Rol yönetimi permission kodlarını içerir.</summary>
    public static class Roles
    {
        public const string View = "Roles.View";
        public const string Query = "Roles.Query";
        public const string Create = "Roles.Create";
        public const string Update = "Roles.Update";
        public const string Delete = "Roles.Delete";
        public const string ManagePermissions = "Roles.ManagePermissions";
    }

    /// <summary>Organizasyon yönetimi permission kodlarını içerir.</summary>
    public static class Organizations
    {
        public const string View = "Organizations.View";
        public const string Query = "Organizations.Query";
        public const string Create = "Organizations.Create";
        public const string Update = "Organizations.Update";
        public const string Delete = "Organizations.Delete";
    }

    /// <summary>Raporlama permission kodlarını içerir.</summary>
    public static class Reports
    {
        public const string View = "Reports.View";
        public const string Query = "Reports.Query";
        public const string Export = "Reports.Export";
        public const string Print = "Reports.Print";
    }

    /// <summary>İnceleme ve soruşturma permission kodlarını içerir.</summary>
    public static class Investigations
    {
        public const string View = "Investigations.View";
        public const string Query = "Investigations.Query";
        public const string Create = "Investigations.Create";
        public const string Update = "Investigations.Update";
        public const string Delete = "Investigations.Delete";
        public const string Approve = "Investigations.Approve";
        public const string Reject = "Investigations.Reject";
        public const string Close = "Investigations.Close";
        public const string Export = "Investigations.Export";
        public const string Print = "Investigations.Print";
    }
}
