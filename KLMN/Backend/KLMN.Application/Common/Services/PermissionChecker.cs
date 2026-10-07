using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Common.Services;

/// <summary>
/// Gerçek endpoint permission kararını DB üzerinden verir.
/// Sıra: ADMIN -> UserPermission override -> RolePermission -> deny.
/// </summary>
internal sealed class PermissionChecker(
    IKLMNDbContext dbContext,
    ICurrentUserService currentUserService)
    : IPermissionChecker
{
    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserService.IsAuthenticated ||
            currentUserService.UserId is not Guid userId)
        {
            return false;
        }

        var isAdmin = await dbContext.UserRoles
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.Role.Code == SystemRoles.Admin,
                cancellationToken);

        if (isAdmin)
        {
            return true;
        }

        var userOverride = await dbContext.UserPermissions
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.Permission.Code == permissionCode)
            .Select(x => (bool?)x.IsGranted)
            .FirstOrDefaultAsync(cancellationToken);

        if (userOverride.HasValue)
        {
            return userOverride.Value;
        }

        return await dbContext.RolePermissions
            .AsNoTracking()
            .AnyAsync(
                rp =>
                    rp.Permission.Code == permissionCode &&
                    dbContext.UserRoles.Any(
                        ur =>
                            ur.UserId == userId &&
                            ur.RoleId == rp.RoleId),
                cancellationToken);
    }
}
