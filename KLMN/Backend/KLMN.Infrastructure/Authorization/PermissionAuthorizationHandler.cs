using KLMN.Application.Common.Interfaces.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace KLMN.Infrastructure.Authorization;

/// <summary>Permission requirement'ını DB-backed checker ile doğrular.</summary>
public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// Kullanıcının gerçek yetkilerini veritabanı üzerinden kontrol eden servistir.
    /// </summary>
    private readonly IPermissionChecker _permissionChecker;

    /// <summary>
    /// permission authorization handler işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public PermissionAuthorizationHandler(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    /// <summary>
    /// Endpoint permission gereksinimini veritabanındaki güncel izinlerle doğrular.
    /// </summary>
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
