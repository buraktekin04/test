using KLMN.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>
/// Controller/action seviyesinde permission policy tanımlamak için kullanılır.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true)]
public sealed class HasPermissionAttribute
    : AuthorizeAttribute
{
    /// <summary>
    /// İstenen permission koduyla dinamik policy oluşturur.
    /// </summary>
    public HasPermissionAttribute(
        string permissionCode)
    {
        Policy =
            AuthorizationPolicyNames.PermissionPrefix +
            permissionCode;
    }
}
