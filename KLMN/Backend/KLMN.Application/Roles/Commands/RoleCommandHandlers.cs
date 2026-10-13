using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Commands;

/// <summary>Rol işlemlerinin ortak isim, benzersizlik ve korunan rol kurallarıdır.</summary>
internal static class RoleRules
{
    /// <summary>Sistem tarafından seed edilen rol adlarına müdahaleyi sınırlar.</summary>
    public static void EnsureEditable(Role role)
    {
        if (role.IsSystemRole || role.Code == SystemRoles.Admin ||
            role.Code == SystemRoles.StandardUser)
        {
            throw new RoleConflictException(
                "Sistem rolleri değiştirilemez, pasifleştirilemez veya silinemez.");
        }
    }

    /// <summary>Normalleştirilmiş teknik rol kodunu üretir.</summary>
    public static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    /// <summary>Aktif ve pasif rollerde benzersiz ad/kod çatışmasını engeller.</summary>
    public static async Task EnsureUniqueAsync(
        IKLMNDbContext db, string name, string code, Guid? exceptId,
        CancellationToken cancellationToken)
    {
        var nameKey = name.Trim().ToUpperInvariant();
        var codeKey = NormalizeCode(code);

        if (codeKey is SystemRoles.Admin or SystemRoles.StandardUser)
            throw new RoleConflictException("Bu rol kodu sistem tarafından ayrılmıştır.");

        var exists = await db.Roles.IgnoreQueryFilters()
            .AnyAsync(x => !x.IsDeleted && (!exceptId.HasValue || x.Id != exceptId.Value) &&
                (x.Name.ToUpper() == nameKey || x.Code.ToUpper() == codeKey),
                cancellationToken);

        if (exists)
            throw new RoleConflictException("Aynı ada veya koda sahip bir rol zaten mevcut.");
    }

    /// <summary>Soft delete yapılmamış rolü concurrency güncellemesine hazır getirir.</summary>
    public static async Task<Role> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var role = await db.Roles.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new RoleNotFoundException();

        if (role.Version != version)
            throw new DbUpdateConcurrencyException(
                "Rol başka bir işlemde güncellendi. Güncel kaydı yeniden açınız.");

        return role;
    }
}

/// <summary>Yeni bir özel rol oluşturur.</summary>
public sealed class CreateRoleCommandHandler(IKLMNDbContext dbContext)
    : IRequestHandler<CreateRoleCommand, RoleMutationResponse>
{
    /// <summary>Korunan kodlarla çakışmayan yeni rol oluşturur.</summary>
    public async Task<RoleMutationResponse> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        await RoleRules.EnsureUniqueAsync(dbContext, request.Name, request.Code, null, ct);
        var role = new Role
        {
            Name = request.Name.Trim(),
            Code = RoleRules.NormalizeCode(request.Code),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null : request.Description.Trim(),
            IsSystemRole = false
        };

        await dbContext.Roles.AddAsync(role, ct);
        await dbContext.SaveChangesAsync(ct);
        return new RoleMutationResponse(role.Id, role.Version);
    }
}

/// <summary>Sistem dışı rol alanlarını xmin denetimiyle düzenler.</summary>
public sealed class UpdateRoleCommandHandler(IKLMNDbContext dbContext)
    : IRequestHandler<UpdateRoleCommand, RoleMutationResponse>
{
    /// <summary>Var olan rolün adını, kodunu ve açıklamasını günceller.</summary>
    public async Task<RoleMutationResponse> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var role = await RoleRules.LoadForChangeAsync(dbContext, request.Id, request.Version, ct);
        RoleRules.EnsureEditable(role);
        await RoleRules.EnsureUniqueAsync(dbContext, request.Name, request.Code, role.Id, ct);

        role.Name = request.Name.Trim();
        role.Code = RoleRules.NormalizeCode(request.Code);
        role.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null : request.Description.Trim();

        await dbContext.SaveChangesAsync(ct);
        return new RoleMutationResponse(role.Id, role.Version);
    }
}

/// <summary>Özel rolün aktiflik durumunu değiştirir.</summary>
public sealed class ChangeRoleStatusCommandHandler(IKLMNDbContext dbContext)
    : IRequestHandler<ChangeRoleStatusCommand, RoleMutationResponse>
{
    /// <summary>Sistem rollerini koruyarak aktiflik değiştirir.</summary>
    public async Task<RoleMutationResponse> Handle(ChangeRoleStatusCommand request, CancellationToken ct)
    {
        var role = await RoleRules.LoadForChangeAsync(dbContext, request.Id, request.Version, ct);
        RoleRules.EnsureEditable(role);
        role.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(ct);
        return new RoleMutationResponse(role.Id, role.Version);
    }
}

/// <summary>Kullanıcı ataması olmayan özel rolü soft-delete yapar.</summary>
public sealed class DeleteRoleCommandHandler(IKLMNDbContext dbContext)
    : IRequestHandler<DeleteRoleCommand>
{
    /// <summary>Sistem rolü veya atanmış rol silinemez.</summary>
    public async Task Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        var role = await RoleRules.LoadForChangeAsync(dbContext, request.Id, request.Version, ct);
        RoleRules.EnsureEditable(role);

        var assigned = await dbContext.UserRoles.IgnoreQueryFilters()
            .AnyAsync(x => x.RoleId == role.Id && !x.IsDeleted, ct);

        if (assigned)
            throw new RoleConflictException(
                "Bu rol bir veya daha fazla kullanıcıya atanmış. Önce kullanıcı rol atamalarını kaldırınız.");

        // Soft-delete interceptor'ı IsDeleted/IsActive/audit alanlarını doldurur.
        dbContext.Roles.Remove(role);
        await dbContext.SaveChangesAsync(ct);
    }
}

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
