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
    public static string CreatePermissionPolicy(params string[] permissionCodes)
    {
        if (permissionCodes is null || permissionCodes.Length == 0 ||
            permissionCodes.Any(string.IsNullOrWhiteSpace) ||
            permissionCodes.Any(code => code.Contains('|')))
        {
            throw new ArgumentException("En az bir geçerli permission kodu gereklidir.", nameof(permissionCodes));
        }

        // Bir policy içindeki birden fazla kod birbirinin alternatifi olarak değerlendirilir.
        return $"{PermissionPrefix}{string.Join("|", permissionCodes.Distinct(StringComparer.OrdinalIgnoreCase))}";
    }
}
