using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Queries.GetRolePermissionOptions;

/// <summary>Modül ve sıra numarasına göre permission seçeneklerini döndürür.</summary>
public sealed class GetRolePermissionOptionsQueryHandler(IKLMNDbContext dbContext)
    : IRequestHandler<GetRolePermissionOptionsQuery, IReadOnlyList<RolePermissionOptionResponse>>
{
    /// <summary>Yetki seçenekleri için yalnızca görüntülenecek alanları seçer.</summary>
    public async Task<IReadOnlyList<RolePermissionOptionResponse>> Handle(
        GetRolePermissionOptionsQuery request, CancellationToken cancellationToken) =>
        await dbContext.Permissions.AsNoTracking()
            .OrderBy(x => x.Module).ThenBy(x => x.SortOrder).ThenBy(x => x.Name)
            .Select(x => new RolePermissionOptionResponse(
                x.Id, x.Code, x.Name, x.Module, x.Description, x.SortOrder))
            .ToListAsync(cancellationToken);
}
