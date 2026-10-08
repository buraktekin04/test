using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>Tek permission kodu gerektiren authorization requirement'tır.</summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);
        PermissionCode = permissionCode;
    }

    public string PermissionCode { get; }
}
