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
        var items = await GetRolesSorting.Definition.Apply(roles, request.SortField, request.SortOrder)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(RoleResponse.Projection)
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
