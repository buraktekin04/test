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
    private readonly KLMNDbContext _dbContext;
    private readonly PermissionSeeder _permissionSeeder;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IConfiguration _configuration;

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

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _permissionSeeder.SeedAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await AssignAdminPermissionsAsync(cancellationToken);
        await SeedInitialAdminAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
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

    private async Task AssignAdminPermissionsAsync(CancellationToken cancellationToken)
    {
        var adminRole = await _dbContext.Roles
            .SingleAsync(x => x.Code == SystemRoles.Admin, cancellationToken);

        var permissions = await _dbContext.Permissions
            .ToListAsync(cancellationToken);

        var existing = await _dbContext.RolePermissions
            .IgnoreQueryFilters()
            .Where(x => x.RoleId == adminRole.Id)
            .ToListAsync(cancellationToken);

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

    private async Task SeedInitialAdminAsync(CancellationToken cancellationToken)
    {
        var userName = _configuration["InitialAdmin:UserName"];
        var email = _configuration["InitialAdmin:Email"];
        var password = _configuration["InitialAdmin:Password"];

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var normalizedUserName = Normalize(userName);
        var normalizedEmail = Normalize(email);

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

        var adminRole = await _dbContext.Roles
            .SingleAsync(x => x.Code == SystemRoles.Admin, cancellationToken);

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

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();
}
