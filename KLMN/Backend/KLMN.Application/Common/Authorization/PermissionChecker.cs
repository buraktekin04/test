using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Common.Authorization;

/// <summary>
/// Permission önceliğini ADMIN -> UserPermission override -> RolePermission -> deny
/// sırasıyla uygular.
/// </summary>
public sealed class PermissionChecker : IPermissionChecker
{
    private readonly IKLMNDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public PermissionChecker(
        IKLMNDbContext dbContext,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionCode) ||
            _currentUserService.UserId is not Guid userId)
        {
            return false;
        }

        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        var userState = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new
            {
                x.IsLocked,
                x.LockoutEnd,
                x.ValidFrom,
                x.ValidTo
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (userState is null)
        {
            return false;
        }

        if (userState.IsLocked &&
            (!userState.LockoutEnd.HasValue ||
             userState.LockoutEnd.Value > utcNow))
        {
            return false;
        }

        if ((userState.ValidFrom.HasValue && userState.ValidFrom.Value > utcNow) ||
            (userState.ValidTo.HasValue && userState.ValidTo.Value <= utcNow))
        {
            return false;
        }

        var isAdmin = await _dbContext.UserRoles
            .AsNoTracking()
            .AnyAsync(
                x => x.UserId == userId &&
                     x.Role.Code == SystemRoles.Admin,
                cancellationToken);

        if (isAdmin)
        {
            return true;
        }

        var userOverride = await _dbContext.UserPermissions
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.Permission.Code == permissionCode)
            .Select(x => (bool?)x.IsGranted)
            .SingleOrDefaultAsync(cancellationToken);

        if (userOverride.HasValue)
        {
            return userOverride.Value;
        }

        return await _dbContext.RolePermissions
            .AsNoTracking()
            .AnyAsync(
                rp =>
                    rp.Permission.Code == permissionCode &&
                    _dbContext.UserRoles.Any(
                        ur =>
                            ur.UserId == userId &&
                            ur.RoleId == rp.RoleId),
                cancellationToken);
    }
}
