using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Queries.GetRoleById;

/// <summary>Pasif roller dahil, soft-delete edilmemiş rol detayını döndürür.</summary>
public sealed class GetRoleByIdQueryHandler(IKLMNDbContext dbContext)
    : IRequestHandler<GetRoleByIdQuery, RoleResponse>
{
    /// <summary>Kimlik ile projeksiyon yapar; bulunamazsa 404 oluşturur.</summary>
    public async Task<RoleResponse> Handle(
        GetRoleByIdQuery request, CancellationToken cancellationToken) =>
        await dbContext.Roles.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(role =>
        new RoleResponse
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsActive = role.IsActive,
            CreatedDate = role.CreatedDate,
            Version = role.Version,
            UserCount = role.UserRoles.Count(x => x.IsActive && !x.IsDeleted &&
                x.User.IsActive && !x.User.IsDeleted),
            PermissionCodes = role.RolePermissions
                .Where(x => x.IsActive && !x.IsDeleted &&
                    x.Permission.IsActive && !x.Permission.IsDeleted)
                .Select(x => x.Permission.Code)
                .OrderBy(x => x)
                .ToArray()
        })
            .SingleOrDefaultAsync(cancellationToken)
        ?? throw new RoleNotFoundException();
}
