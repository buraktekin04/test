using KLMN.Application.Roles.Commands.Common;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Commands.DeleteRole;

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
