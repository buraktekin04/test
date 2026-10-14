using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>En az bir permission kodu gerektiren authorization requirement'tır (OR).</summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// permission requirement işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public PermissionRequirement(params string[] permissionCodes)
    {
        if (permissionCodes is null || permissionCodes.Length == 0 ||
            permissionCodes.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("En az bir permission kodu gereklidir.", nameof(permissionCodes));
        }

        PermissionCodes = permissionCodes;
    }

    /// <summary>En az bir tanesi sağlandığında yetki verilen alternatif permission kodlarıdır.</summary>
    public IReadOnlyList<string> PermissionCodes { get; }

    /// <summary>Tek permission kullanan mevcut kodlarla geriye dönük uyumluluk.</summary>
    public string PermissionCode => PermissionCodes[0];
}
