using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Common.Authorization;

/// <summary>
/// Rol permission'ları ve kullanıcı override kayıtlarını değerlendirerek
/// effective permission setini hesaplar.
/// </summary>
public sealed class UserAuthorizationService : IUserAuthorizationService
{
    /// <summary>
    /// EF Core üzerinden veriye erişimi sağlayan Application katmanı veritabanı sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;

    /// <summary>
    /// user authorization service işlemini uygulama kurallarına göre gerçekleştirir.
    /// </summary>
    public UserAuthorizationService(IKLMNDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// get async işlemini uygulama kurallarına göre gerçekleştirir.
    /// </summary>
    public async Task<UserAuthorizationSnapshot> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Kullanıcının atandığı geçerli rol kodlarıdır.
        var roles = await _dbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Role.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

        // ADMIN rolü ile tam yetki sahibi olma durumudur.
        var isAdmin = roles.Contains(
            SystemRoles.Admin,
            StringComparer.OrdinalIgnoreCase);

        if (isAdmin)
        {
            // Yönetici rolünün erişebileceği tüm etkin permission kodlarıdır.
            var allPermissions = await _dbContext.Permissions
                .AsNoTracking()
                .Select(x => x.Code)
                .Distinct()
                .ToListAsync(cancellationToken);

            return new UserAuthorizationSnapshot
            {
                Roles = roles,
                Permissions = allPermissions,
                IsAdmin = true
            };
        }

        // Kullanıcının rollerinden hesaplanan başlangıç izin listesidir.
        var rolePermissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp =>
                _dbContext.UserRoles.Any(ur =>
                    ur.UserId == userId &&
                    ur.RoleId == rp.RoleId))
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Kullanıcı override'larının da uygulandığı nihai izin kümesidir.
        var effectivePermissions = new HashSet<string>(
            rolePermissions,
            StringComparer.OrdinalIgnoreCase);

        // overrides değerini ilgili kontrol ve işlem adımlarında kullanılmak üzere hesaplar.
        var overrides = await _dbContext.UserPermissions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Permission.Code, x.IsGranted })
            .ToListAsync(cancellationToken);

        foreach (var item in overrides)
        {
            if (item.IsGranted)
            {
                effectivePermissions.Add(item.Code);
            }
            else
            {
                effectivePermissions.Remove(item.Code);
            }
        }

        return new UserAuthorizationSnapshot
        {
            Roles = roles,
            Permissions = effectivePermissions.OrderBy(x => x).ToArray(),
            IsAdmin = false
        };
    }
}
