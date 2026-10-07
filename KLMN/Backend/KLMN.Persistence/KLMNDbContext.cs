using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Entities.Identity;
using KLMN.Domain.Entities.Organization;
using KLMN.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence;

/// <summary>
/// KLMN PostgreSQL veritabanı için ana EF Core DbContext sınıfıdır.
/// </summary>
public sealed class KLMNDbContext(
    DbContextOptions<KLMNDbContext> options)
    : DbContext(options),
      IKLMNDbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions =>
        Set<RolePermission>();

    public DbSet<UserPermission> UserPermissions =>
        Set<UserPermission>();

    public DbSet<RefreshToken> RefreshTokens =>
        Set<RefreshToken>();

    public DbSet<PasswordResetToken> PasswordResetTokens =>
        Set<PasswordResetToken>();

    public DbSet<OrganizationUnit> OrganizationUnits =>
        Set<OrganizationUnit>();

    /// <inheritdoc />
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("klmn");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(KLMNDbContext).Assembly);

        modelBuilder.ApplyKLMNGlobalQueryFilters();
    }
}
