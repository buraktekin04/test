using KLMN.Application.Roles.Commands.Common;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Commands.UpdateRolePermissions;

/// <summary>Role ait izin kümesini mevcut ilişki kayıtlarını yeniden kullanarak günceller.</summary>
public sealed class UpdateRolePermissionsCommandHandler(
    IKLMNDbContext dbContext,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateRolePermissionsCommand, RoleMutationResponse>
{
    /// <summary>Aktif izinleri doğrular; revokeleri soft-delete ederek role ait xmin'i yeniler.</summary>
    public async Task<RoleMutationResponse> Handle(
        UpdateRolePermissionsCommand request, CancellationToken ct)
    {
        var role = await RoleRules.LoadForChangeAsync(dbContext, request.Id, request.Version, ct);
        RoleRules.EnsureEditable(role);

        // İstemciden gelen her permission'ın aktif ve mevcut olduğundan emin olunur.
        var requestedIds = request.PermissionIds.ToHashSet();
        var validIds = await dbContext.Permissions.AsNoTracking()
            .Where(x => requestedIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (validIds.Count != requestedIds.Count)
            throw new RoleConflictException(
                "Seçilen izinlerden biri bulunamadı veya aktif değil. Listeyi güncelleyiniz.");

        // Önceden kaldırılmış ilişkiler de yüklenir; aynı (RoleId, PermissionId)
        // çifti yeniden aktif edilirken unique index ihlal edilmez.
        var relations = await dbContext.RolePermissions.IgnoreQueryFilters()
            .Where(x => x.RoleId == role.Id)
            .ToListAsync(ct);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var byPermission = relations.ToDictionary(x => x.PermissionId);
        var changed = false;

        foreach (var relation in relations)
        {
            var shouldGrant = requestedIds.Contains(relation.PermissionId);
            if (shouldGrant && (relation.IsDeleted || !relation.IsActive))
            {
                relation.IsDeleted = false;
                relation.IsActive = true;
                relation.DeletedDate = null;
                relation.DeletedBy = null;
                changed = true;
            }
            else if (!shouldGrant && !relation.IsDeleted)
            {
                relation.IsActive = false;
                relation.IsDeleted = true;
                relation.DeletedDate = now;
                relation.DeletedBy = currentUserService.UserId;
                changed = true;
            }
        }

        foreach (var permissionId in requestedIds)
        {
            if (byPermission.ContainsKey(permissionId)) continue;
            await dbContext.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            }, ct);
            changed = true;
        }

        if (changed)
        {
            // İlişki değişikliği tek başına rolün xmin değerini değiştirmez;
            // rolü de güncelleyerek aynı anda iki yetki ekranı kaydını 409'a dönüştürür.
            role.UpdatedDate = now;
            await dbContext.SaveChangesAsync(ct);
        }

        return new RoleMutationResponse(role.Id, role.Version);
    }
}
