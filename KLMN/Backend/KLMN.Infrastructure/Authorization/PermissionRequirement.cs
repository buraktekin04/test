using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>Tek permission kodu gerektiren authorization requirement'tır.</summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// permission requirement işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public PermissionRequirement(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);
        PermissionCode = permissionCode;
    }

    /// <summary>
    /// Authorization gereksiniminin kontrol edeceği teknik izin kodudur.
    /// </summary>
    public string PermissionCode { get; }
}
