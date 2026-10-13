using KLMN.Application.Roles.Commands.Common;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Commands.UpdateRole;

/// <summary>Sistem dışı rol alanlarını xmin denetimiyle düzenler.</summary>
public sealed class UpdateRoleCommandHandler(IKLMNDbContext dbContext)
    : IRequestHandler<UpdateRoleCommand, RoleMutationResponse>
{
    /// <summary>Var olan rolün adını, kodunu ve açıklamasını günceller.</summary>
    public async Task<RoleMutationResponse> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var role = await RoleRules.LoadForChangeAsync(dbContext, request.Id, request.Version, ct);
        RoleRules.EnsureEditable(role);
        await RoleRules.EnsureUniqueAsync(dbContext, request.Name, request.Code, role.Id, ct);

        role.Name = request.Name.Trim();
        role.Code = RoleRules.NormalizeCode(request.Code);
        role.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null : request.Description.Trim();

        await dbContext.SaveChangesAsync(ct);
        return new RoleMutationResponse(role.Id, role.Version);
    }
}
