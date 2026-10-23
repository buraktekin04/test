using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Roles.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Queries.GetRoles;

/// <summary>Role listesi için EF Core filtre, toplam kayıt ve projection işlemlerini yürütür.</summary>
public sealed class GetRolesQueryHandler(IKLMNDbContext dbContext)
    : IRequestHandler<GetRolesQuery, PagedResult<RoleResponse>>
{
    /// <summary>Aktiflik ve sistem rolü filtreleriyle sayfalı rol listesini döndürür.</summary>
    public async Task<PagedResult<RoleResponse>> Handle(
        GetRolesQuery request, CancellationToken cancellationToken)
    {
        // Pasif roller istenirse global ActiveFilter yerine soft-delete korumalı sorgu yapılır.
        var roles = request.IncludeInactive || request.IsActive == false
            ? dbContext.Roles.IgnoreQueryFilters().Where(x => !x.IsDeleted)
            : dbContext.Roles.AsQueryable();

        roles = roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            roles = roles.Where(x =>
                x.Name.ToUpper().Contains(search) ||
                x.Code.ToUpper().Contains(search) ||
                (x.Description != null && x.Description.ToUpper().Contains(search)));
        }

        if (request.IsActive.HasValue)
            roles = roles.Where(x => x.IsActive == request.IsActive.Value);

        if (request.IsSystemRole.HasValue)
            roles = roles.Where(x => x.IsSystemRole == request.IsSystemRole.Value);

        var count = await roles.CountAsync(cancellationToken);
        var items = await roles
            .OrderBy(x => x.IsSystemRole ? 0 : 1)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return new PagedResult<RoleResponse>
        {
            Items = items,
            TotalCount = count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
