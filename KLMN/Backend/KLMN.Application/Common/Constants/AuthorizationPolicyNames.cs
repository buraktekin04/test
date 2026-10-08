namespace KLMN.Application.Common.Constants;

/// <summary>Dinamik permission policy adlandırma kurallarını tanımlar.</summary>
public static class AuthorizationPolicyNames
{
    public const string PermissionPrefix = "Permission:";

    public static string CreatePermissionPolicy(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);
        return $"{PermissionPrefix}{permissionCode}";
    }
}
