using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KLMN.Persistence.Seeds;

/// <summary>
/// Sistem rollerini, ADMIN permission ilişkilerini ve ilk yönetici hesabını seed eder.
/// Mevcut admin parolası startup sırasında değiştirilmez.
/// </summary>
internal sealed class IdentitySeeder
{
    /// <summary>
    /// EF Core üzerinden identity, rol ve permission kayıtlarına erişen PostgreSQL DbContext'tir.
    /// </summary>
    private readonly KLMNDbContext _dbContext;
    /// <summary>
    /// Başlangıç yetki kodlarını veritabanıyla eşitleyen seed servisidir.
    /// </summary>
    private readonly PermissionSeeder _permissionSeeder;
    /// <summary>
    /// İlk yönetici hesabına ait parolayı hashlemek için kullanılan servistir.
    /// </summary>
    private readonly IPasswordHasherService _passwordHasherService;
    /// <summary>
    /// Güvenli yapılandırmadan başlangıç yönetici bilgilerini okuyan sağlayıcıdır.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// identity seeder işlemini ilgili persistence sorumluluğuyla yerine getirir.
    /// </summary>
    public IdentitySeeder(
        KLMNDbContext dbContext,
        PermissionSeeder permissionSeeder,
        IPasswordHasherService passwordHasherService,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _permissionSeeder = permissionSeeder;
        _passwordHasherService = passwordHasherService;
        _configuration = configuration;
    }

    /// <summary>
    /// Merkezi rollerin ve izinlerin idempotent veritabanı başlangıç kayıtlarını oluşturur.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _permissionSeeder.SeedAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await AssignAdminPermissionsAsync(cancellationToken);
        await SeedInitialAdminAsync(cancellationToken);
    }

    /// <summary>
    /// Var olan rol kodlarını koruyarak başlangıç rollerini ekler veya yeniden etkinleştirir.
    /// </summary>
    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        // Soft delete edilmiş kayıtlar dahil kodla eşleşen mevcut rollerin sözlüğüdür.
        var existingRoles = await _dbContext.Roles
            .IgnoreQueryFilters()
            .ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var seedItem in IdentitySeedData.Roles)
        {
            if (existingRoles.TryGetValue(seedItem.Code, out var role))
            {
                role.Name = seedItem.Name;
                role.Description = seedItem.Description;
                role.IsSystemRole = seedItem.IsSystemRole;
                role.IsActive = true;
                role.IsDeleted = false;
                role.DeletedDate = null;
                role.DeletedBy = null;
                continue;
            }

            await _dbContext.Roles.AddAsync(
                new Role
                {
                    Name = seedItem.Name,
                    Code = seedItem.Code,
                    Description = seedItem.Description,
                    IsSystemRole = seedItem.IsSystemRole
                },
                cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// ADMIN rolüne sistemde tanımlı tüm güncel yetki kodlarını ilişkilendirir.
    /// </summary>
    private async Task AssignAdminPermissionsAsync(CancellationToken cancellationToken)
    {
        // Sistem genelinde tam yetkili ADMIN rolünün veritabanı kaydıdır.
        var adminRole = await _dbContext.Roles
            .SingleAsync(x => x.Code == SystemRoles.Admin, cancellationToken);

        // ADMIN rolüne atanması gereken güncel sistem izinlerini içerir.
        var permissions = await _dbContext.Permissions
            .ToListAsync(cancellationToken);

        // Rol izin ilişkilerinin aktif ve silinmiş kayıtlar dahil mevcut listesidir.
        var existing = await _dbContext.RolePermissions
            .IgnoreQueryFilters()
            .Where(x => x.RoleId == adminRole.Id)
            .ToListAsync(cancellationToken);

        // Permission kimliğine göre hızlı erişim sağlayan ilişki sözlüğüdür.
        var byPermission = existing.ToDictionary(x => x.PermissionId);

        foreach (var permission in permissions)
        {
            if (byPermission.TryGetValue(permission.Id, out var relation))
            {
                relation.IsActive = true;
                relation.IsDeleted = false;
                relation.DeletedDate = null;
                relation.DeletedBy = null;
                continue;
            }

            await _dbContext.RolePermissions.AddAsync(
                new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                },
                cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Configuration'dan gelen kimlikle ilk yöneticiyi oluşturur; mevcut parolasını değiştirmez.
    /// </summary>
    private async Task SeedInitialAdminAsync(CancellationToken cancellationToken)
    {
        // Yönetici hesabı için yapılandırmadan alınan kullanıcı adıdır.
        var userName = _configuration["InitialAdmin:UserName"];
        // İlk yönetici hesabının yapılandırılmış e-posta adresidir.
        var email = _configuration["InitialAdmin:Email"];
        // İlk kurulum için yapılandırmadan alınan açık parola; hashlenmeden saklanmaz.
        var password = _configuration["InitialAdmin:Password"];

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        // Kullanıcı adının karşılaştırma için normalize edilmiş versiyonudur.
        var normalizedUserName = Normalize(userName);
        // E-posta adresinin karşılaştırma için normalize edilmiş versiyonudur.
        var normalizedEmail = Normalize(email);

        // İlk kurulum yönetici rolü atanacak mevcut veya yeni kullanıcıdır.
        var adminUser = await _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x =>
                    x.NormalizedUserName == normalizedUserName ||
                    x.NormalizedEmail == normalizedEmail,
                cancellationToken);

        if (adminUser is null)
        {
            adminUser = new User
            {
                UserName = userName.Trim(),
                NormalizedUserName = normalizedUserName,
                Email = email.Trim(),
                NormalizedEmail = normalizedEmail,
                FirstName = _configuration["InitialAdmin:FirstName"]?.Trim() ?? "System",
                LastName = _configuration["InitialAdmin:LastName"]?.Trim() ?? "Administrator",
                PasswordHash = _passwordHasherService.HashPassword(password),
                SecurityStamp = Guid.NewGuid().ToString("N"),
                PasswordChangedDate = DateTime.UtcNow
            };

            await _dbContext.Users.AddAsync(adminUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            adminUser.IsDeleted = false;
            adminUser.IsActive = true;
            adminUser.DeletedDate = null;
            adminUser.DeletedBy = null;
        }

        // Sistem genelinde tam yetkili ADMIN rolünün veritabanı kaydıdır.
        var adminRole = await _dbContext.Roles
            .SingleAsync(x => x.Code == SystemRoles.Admin, cancellationToken);

        // Aynı yöneticiye daha önce atanmış ADMIN rol ilişkisi kaydıdır.
        var existingUserRole = await _dbContext.UserRoles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.UserId == adminUser.Id && x.RoleId == adminRole.Id,
                cancellationToken);

        if (existingUserRole is null)
        {
            await _dbContext.UserRoles.AddAsync(
                new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                },
                cancellationToken);
        }
        else
        {
            existingUserRole.IsActive = true;
            existingUserRole.IsDeleted = false;
            existingUserRole.DeletedDate = null;
            existingUserRole.DeletedBy = null;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Kullanıcı adı ve e-posta karşılaştırmaları için kültürden bağımsız normalleştirme yapar.
    /// </summary>
    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();
}
