using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Entities.Identity;
using KLMN.Domain.Entities.Organization;
using KLMN.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence.Contexts;

/// <summary>KLMN PostgreSQL ana EF Core DbContext sınıfıdır.</summary>
public sealed class KLMNDbContext : DbContext, IKLMNDbContext
{
    public KLMNDbContext(DbContextOptions<KLMNDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(KLMNDbContext).Assembly);

        modelBuilder.ApplyGlobalQueryFilters();
    }
}
