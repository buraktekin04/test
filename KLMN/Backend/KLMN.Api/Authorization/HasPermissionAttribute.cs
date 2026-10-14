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
    public HasPermissionAttribute(params string[] permissionCodes)
    {
        Policy = AuthorizationPolicyNames.CreatePermissionPolicy(permissionCodes);
        Permissions = permissionCodes;
    }

    /// <summary>Endpoint'e erişim için yeterli alternatif permission kodlarıdır (OR).</summary>
    public IReadOnlyList<string> Permissions { get; }

    /// <summary>Tek permission kullanan mevcut kodlarla geriye dönük uyumluluk.</summary>
    public string Permission => Permissions[0];
}
