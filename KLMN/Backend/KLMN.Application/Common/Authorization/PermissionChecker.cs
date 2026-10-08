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
    /// <summary>
    /// EF Core üzerinden veriye erişimi sağlayan Application katmanı veritabanı sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// İstek yapan kullanıcının kimliği ve istemci bilgilerine erişir.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;
    /// <summary>
    /// UTC zamanını sistem saatine doğrudan bağımlı olmadan sağlar.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// permission checker işlemini uygulama kurallarına göre gerçekleştirir.
    /// </summary>
    public PermissionChecker(
        IKLMNDbContext dbContext,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// has permission async işlemini uygulama kurallarına göre gerçekleştirir.
    /// </summary>
    public async Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionCode) ||
            _currentUserService.UserId is not Guid userId)
        {
            return false;
        }

        // Hesap geçerliliği ve token süreleri için ortak UTC zaman değeridir.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        // Kullanıcının kilit ve hesap geçerliliği kontrolünde gereken verileridir.
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

        // ADMIN rolü ile tam yetki sahibi olma durumudur.
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

        // user override değerini ilgili kontrol ve işlem adımlarında kullanılmak üzere hesaplar.
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
