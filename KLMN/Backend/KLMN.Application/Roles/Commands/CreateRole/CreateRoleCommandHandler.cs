using KLMN.Application.Roles.Commands.Common;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Commands.CreateRole;

/// <summary>Yeni bir özel rol oluşturur.</summary>
public sealed class CreateRoleCommandHandler(IKLMNDbContext dbContext)
    : IRequestHandler<CreateRoleCommand, RoleMutationResponse>
{
    /// <summary>Korunan kodlarla çakışmayan yeni rol oluşturur.</summary>
    public async Task<RoleMutationResponse> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        await RoleRules.EnsureUniqueAsync(dbContext, request.Name, request.Code, null, ct);
        var role = new Role
        {
            Name = request.Name.Trim(),
            Code = RoleRules.NormalizeCode(request.Code),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null : request.Description.Trim(),
            IsSystemRole = false
        };

        await dbContext.Roles.AddAsync(role, ct);
        await dbContext.SaveChangesAsync(ct);
        return new RoleMutationResponse(role.Id, role.Version);
    }
}
