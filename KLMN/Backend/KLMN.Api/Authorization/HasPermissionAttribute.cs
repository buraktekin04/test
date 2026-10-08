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
    public HasPermissionAttribute(string permissionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);

        Permission = permissionCode;
        Policy = AuthorizationPolicyNames.CreatePermissionPolicy(permissionCode);
    }

    public string Permission { get; }
}
