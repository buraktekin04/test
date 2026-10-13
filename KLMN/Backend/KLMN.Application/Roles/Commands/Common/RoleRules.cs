using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Commands.Common;

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
