using KLMN.Domain.Entities.Identity;
using KLMN.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Common.Interfaces.Persistence;

/// <summary>Application katmanının KLMN veritabanına erişim sözleşmesidir.</summary>
public interface IKLMNDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserPermission> UserPermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<OrganizationUnit> OrganizationUnits { get; }

    /// <summary>
    /// save changes async işlemini çağıran katmana belirtilen sözleşmeyle sunar.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
