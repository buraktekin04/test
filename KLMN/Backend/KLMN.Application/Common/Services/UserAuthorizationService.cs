using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Common.Services;

/// <summary>
/// Kullanıcının güncel rol ve efektif permission setini DB üzerinden hesaplar.
/// </summary>
internal sealed class UserAuthorizationService(
    IKLMNDbContext dbContext)
    : IUserAuthorizationService
{
    /// <inheritdoc />
    public async Task<UserAuthorizationSnapshot> GetSnapshotAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var roles = await dbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Role.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var isAdmin =
            roles.Contains(
                SystemRoles.Admin,
                StringComparer.OrdinalIgnoreCase);

        if (isAdmin)
        {
            var allPermissions = await dbContext.Permissions
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Code)
                .Select(x => x.Code)
                .Distinct()
                .ToListAsync(cancellationToken);

            return new UserAuthorizationSnapshot(
                roles,
                allPermissions,
                true);
        }

        var rolePermissionCodes = await dbContext.RolePermissions
            .AsNoTracking()
            .Where(x =>
                dbContext.UserRoles.Any(ur =>
                    ur.UserId == userId &&
                    ur.RoleId == x.RoleId))
            .Select(x => x.Permission.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

        var userOverrides = await dbContext.UserPermissions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                x.Permission.Code,
                x.IsGranted
            })
            .ToListAsync(cancellationToken);

        var effective =
            rolePermissionCodes.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        foreach (var userOverride in userOverrides)
        {
            if (userOverride.IsGranted)
            {
                effective.Add(userOverride.Code);
            }
            else
            {
                effective.Remove(userOverride.Code);
            }
        }

        return new UserAuthorizationSnapshot(
            roles,
            effective.OrderBy(x => x).ToArray(),
            false);
    }
}
