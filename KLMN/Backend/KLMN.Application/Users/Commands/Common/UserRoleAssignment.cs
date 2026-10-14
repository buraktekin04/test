using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Commands.Common;

/// <summary>Kullanıcı ve rol değişiklikleri çağıranın tek SaveChanges işlemiyle kaydedilir.</summary>
internal static class UserRoleAssignment
{
    public static async Task ApplyAsync(IKLMNDbContext db, ICurrentUserService currentUser,
        IPermissionChecker permissions, IUserAuthorizationService authorization,
        User user, IReadOnlyCollection<Guid>? requestedIds, bool creating, CancellationToken ct)
    {
        if (requestedIds is null && !creating) return;
        if (requestedIds is null)
        {
            var standard = await db.Roles.Where(r => r.Code == SystemRoles.StandardUser)
                .Select(r => (Guid?)r.Id).SingleOrDefaultAsync(ct);
            if (standard is null)
                throw new UserConflictException("Standart kullanıcı rolü bulunamadı. Seed verilerini kontrol ediniz.");
            requestedIds = [standard.Value];
        }
        else if (!await permissions.HasPermissionAsync(PermissionCodes.Users.AssignRole, ct))
            throw new UserRoleAssignmentForbiddenException("Kullanıcıya rol atama yetkiniz bulunmuyor.");

        var ids = requestedIds.ToHashSet();
        if (ids.Count == 0 || ids.Count > 100 || ids.Contains(Guid.Empty) || ids.Count != requestedIds.Count)
            throw new UserConflictException("En az bir geçerli, benzersiz rol seçiniz (en fazla 100).");
        var links = creating ? new List<UserRole>() : await db.UserRoles.IgnoreQueryFilters()
            .Where(x => x.UserId == user.Id && !x.IsDeleted).ToListAsync(ct);
        var previous = links.Where(x => x.IsActive).Select(x => x.RoleId).ToHashSet();
        if (!creating && previous.SetEquals(ids)) return;
        if (!creating && currentUser.UserId == user.Id)
            throw new UserRoleAssignmentForbiddenException("Kendi rollerinizi değiştiremezsiniz. Başka bir yönetici işlem yapmalıdır.");

        var roles = await db.Roles.IgnoreQueryFilters().Where(r => ids.Contains(r.Id)).ToListAsync(ct);
        if (roles.Count != ids.Count || roles.Any(r => (r.IsDeleted || !r.IsActive) && !previous.Contains(r.Id)))
            throw new UserConflictException("Yeni atanan roller aktif ve silinmemiş olmalıdır. Rol listesini yenileyiniz.");
        var adminIds = await db.Roles.IgnoreQueryFilters().Where(r => r.Code == SystemRoles.Admin)
            .Select(r => r.Id).ToListAsync(ct);
        if (adminIds.Any(id => previous.Contains(id) != ids.Contains(id)))
        {
            var actor = currentUser.UserId.HasValue
                ? await authorization.GetAsync(currentUser.UserId.Value, ct) : null;
            if (actor?.IsAdmin != true)
                throw new UserRoleAssignmentForbiddenException("ADMIN rolünü yalnızca yönetici atayabilir veya kaldırabilir.");
        }
        foreach (var link in links) link.IsActive = ids.Contains(link.RoleId);
        foreach (var id in ids.Where(id => links.All(link => link.RoleId != id)))
            await db.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = id }, ct);
        // Rol değişimi xmin sürümünü ilerletir ve eski access token'ları geçersiz kılar.
        user.SecurityStamp = Guid.NewGuid().ToString("N");
    }
}
