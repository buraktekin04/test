using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>
/// Tek bir permission kodunu gerektiren authorization requirement'tır.
/// </summary>
public sealed class PermissionRequirement(
    string permissionCode)
    : IAuthorizationRequirement
{
    /// <summary>
    /// Endpoint için gerekli permission kodudur.
    /// </summary>
    public string PermissionCode { get; } =
        permissionCode;
}
