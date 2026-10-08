namespace KLMN.Application.Common.Constants;

/// <summary>Dinamik permission policy adlandırma kurallarını tanımlar.</summary>
public static class AuthorizationPolicyNames
{
    /// <summary>
    /// Dinamik yetkilendirme policy'lerinin sabit ön ekidir.
    /// </summary>
    public const string PermissionPrefix = "Permission:";

    /// <summary>
    /// Verilen permission kodunu dinamik authorization policy adına dönüştürür.
    /// </summary>
    public static string CreatePermissionPolicy(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);
        return $"{PermissionPrefix}{permissionCode}";
    }
}
