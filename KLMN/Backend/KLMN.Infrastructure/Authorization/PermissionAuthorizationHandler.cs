using KLMN.Application.Common.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>
/// Permission requirement'ını IPermissionChecker üzerinden DB-backed olarak doğrular.
/// </summary>
internal sealed class PermissionAuthorizationHandler(
    IPermissionChecker permissionChecker)
    : AuthorizationHandler<PermissionRequirement>
{
    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (await permissionChecker.HasPermissionAsync(
                requirement.PermissionCode))
        {
            context.Succeed(requirement);
        }
    }
}
