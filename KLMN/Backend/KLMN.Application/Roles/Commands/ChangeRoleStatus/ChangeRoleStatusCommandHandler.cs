using KLMN.Application.Roles.Commands.Common;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Commands.ChangeRoleStatus;

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
