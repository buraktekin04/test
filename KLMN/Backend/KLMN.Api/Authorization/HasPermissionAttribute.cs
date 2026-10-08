using KLMN.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;

namespace KLMN.Api.Authorization;

/// <summary>Controller/action üzerinde permission policy tanımlar.</summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// has permission attribute işlemini ilgili katmanın sorumluluğuna göre gerçekleştirir.
    /// </summary>
    public HasPermissionAttribute(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);

        Permission = permissionCode;
        Policy = AuthorizationPolicyNames.CreatePermissionPolicy(permissionCode);
    }

    /// <summary>
    /// İlgili endpoint için aranan yetki kodudur.
    /// </summary>
    public string Permission { get; }
}
