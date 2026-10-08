using KLMN.Application.Common.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>Permission requirement'ını DB-backed checker ile doğrular.</summary>
public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionChecker _permissionChecker;

    public PermissionAuthorizationHandler(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        if (await _permissionChecker.HasPermissionAsync(requirement.PermissionCode))
        {
            context.Succeed(requirement);
        }
    }
}
