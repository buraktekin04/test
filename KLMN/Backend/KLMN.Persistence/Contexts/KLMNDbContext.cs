using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Entities.Identity;
using KLMN.Domain.Entities.Organization;
using KLMN.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence.Contexts;

/// <summary>KLMN PostgreSQL ana EF Core DbContext sınıfıdır.</summary>
public sealed class KLMNDbContext : DbContext, IKLMNDbContext
{
    /// <summary>
    /// klmndb context işlemini ilgili persistence sorumluluğuyla yerine getirir.
    /// </summary>
    public KLMNDbContext(DbContextOptions<KLMNDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Kullanıcı hesaplarına ait EF Core DbSet koleksiyonudur.
    /// </summary>
    public DbSet<User> Users => Set<User>();
    /// <summary>
    /// Tanımlanan dinamik ve sistem rollerinin EF Core DbSet koleksiyonudur.
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();
    /// <summary>
    /// Uygulamanın başlangıçta tanımlanan permission seed kayıtlarını içerir.
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();
    /// <summary>
    /// Kullanıcılarla roller arasındaki ilişki kayıtlarının DbSet koleksiyonudur.
    /// </summary>
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    /// <summary>
    /// Role atanmış izin ilişki kayıtlarının DbSet koleksiyonudur.
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    /// <summary>
    /// Kullanıcı bazlı izin verme veya engelleme override kayıtlarının DbSet'idir.
    /// </summary>
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    /// <summary>
    /// Kullanıcı oturumları için hash olarak saklanan refresh token kayıtlarının DbSet'idir.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    /// <summary>
    /// Tek kullanımlık parola sıfırlama token kayıtlarının DbSet'idir.
    /// </summary>
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    /// <summary>
    /// Hiyerarşik organizasyon birimlerine ait DbSet koleksiyonudur.
    /// </summary>
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();

    /// <summary>
    /// Entity konfigürasyonlarını assembly'den yükleyip global sorgu filtrelerini uygular.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(KLMNDbContext).Assembly);

        modelBuilder.ApplyGlobalQueryFilters();
    }
}
