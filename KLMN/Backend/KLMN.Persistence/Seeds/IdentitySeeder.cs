using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Security;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KLMN.Persistence.Seeds;

/// <summary>
/// Sistem rollerini ve ilk ADMIN kullanıcısını seed eder.
/// Mevcut ADMIN parolası seed sırasında değiştirilmez.
/// </summary>
internal sealed class IdentitySeeder(
    IKLMNDbContext dbContext,
    IPasswordHasherService passwordHasherService,
    IOptions<InitialAdminSettings> adminOptions)
{
    /// <summary>
    /// Rol ve ilk yönetici seed işlemlerini çalıştırır.
/// </summary>
    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var adminRole =
            await EnsureRoleAsync(
                SystemRoles.Admin,
                "Administrator",
                true,
                cancellationToken);

        await EnsureRoleAsync(
            SystemRoles.StandardUser,
            "Standard User",
            true,
            cancellationToken);

        await EnsureRoleAsync(
            SystemRoles.InvestigationOfficer,
            "Investigation Officer",
            false,
            cancellationToken);

        await EnsureRoleAsync(
            SystemRoles.BranchManager,
            "Branch Manager",
            false,
            cancellationToken);

        await EnsureAdminPermissionsAsync(
            adminRole,
            cancellationToken);

        await EnsureInitialAdminAsync(
            adminRole,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Role> EnsureRoleAsync(
        string code,
        string name,
        bool isSystemRole,
        CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);

        if (role is not null)
        {
            if (role.IsDeleted)
            {
                role.IsDeleted = false;
                role.IsActive = true;
                role.DeletedDate = null;
                role.DeletedBy = null;
            }

            role.IsSystemRole = isSystemRole;

            return role;
        }

        role = new Role
        {
            Code = code,
            Name = name,
            IsSystemRole = isSystemRole
        };

        dbContext.Roles.Add(role);

        return role;
    }

    private async Task EnsureAdminPermissionsAsync(
        Role adminRole,
        CancellationToken cancellationToken)
    {
        var permissionIds =
            await dbContext.Permissions
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

        var assigned =
            await dbContext.RolePermissions
                .IgnoreQueryFilters()
                .Where(x => x.RoleId == adminRole.Id)
                .ToListAsync(cancellationToken);

        var assignedByPermission =
            assigned.ToDictionary(
                x => x.PermissionId);

        foreach (var permissionId in permissionIds)
        {
            if (assignedByPermission.TryGetValue(
                    permissionId,
                    out var existing))
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.IsActive = true;
                    existing.DeletedDate = null;
                    existing.DeletedBy = null;
                }

                continue;
            }

            dbContext.RolePermissions.Add(
                new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permissionId
                });
        }
    }

    private async Task EnsureInitialAdminAsync(
        Role adminRole,
        CancellationToken cancellationToken)
    {
        var settings = adminOptions.Value;

        if (string.IsNullOrWhiteSpace(settings.UserName) ||
            string.IsNullOrWhiteSpace(settings.Email) ||
            string.IsNullOrWhiteSpace(settings.Password))
        {
            return;
        }

        var normalizedUserName =
            settings.UserName.Trim().ToUpperInvariant();

        var normalizedEmail =
            settings.Email.Trim().ToUpperInvariant();

        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x =>
                    x.NormalizedUserName == normalizedUserName ||
                    x.NormalizedEmail == normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            user = new User
            {
                UserName = settings.UserName.Trim(),
                NormalizedUserName = normalizedUserName,
                Email = settings.Email.Trim(),
                NormalizedEmail = normalizedEmail,
                FirstName = settings.FirstName.Trim(),
                LastName = settings.LastName.Trim(),
                PasswordHash =
                    passwordHasherService.HashPassword(
                        settings.Password),
                PasswordChangedDate = DateTime.UtcNow
            };

            dbContext.Users.Add(user);
        }
        else if (user.IsDeleted)
        {
            user.IsDeleted = false;
            user.IsActive = true;
            user.DeletedDate = null;
            user.DeletedBy = null;
        }

        var hasAdminRole =
            await dbContext.UserRoles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == user.Id &&
                        x.RoleId == adminRole.Id,
                    cancellationToken);

        if (hasAdminRole is null)
        {
            dbContext.UserRoles.Add(
                new UserRole
                {
                    UserId = user.Id,
                    RoleId = adminRole.Id
                });
        }
        else if (hasAdminRole.IsDeleted)
        {
            hasAdminRole.IsDeleted = false;
            hasAdminRole.IsActive = true;
            hasAdminRole.DeletedDate = null;
            hasAdminRole.DeletedBy = null;
        }
    }
}
