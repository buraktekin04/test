using KLMN.Application.Common.Constants;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>
/// Kullanıcı listeleme sorgusunu EF Core üzerinden çalıştırır.
/// </summary>
internal sealed class GetUsersQueryHandler(
    IKLMNDbContext dbContext)
    : IRequestHandler<GetUsersQuery, PagedResult<UserListItemResponse>>
{
    public async Task<PagedResult<UserListItemResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            dbContext.Users
                .AsNoTracking();

        if (request.IncludeInactive)
        {
            query = query.IgnoreQueryFilters(
                [QueryFilterNames.ActiveFilter]);
        }

        var search = request.Search?.Trim();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch =
                search.ToUpperInvariant();

            query = query.Where(x =>
                x.NormalizedUserName.Contains(normalizedSearch) ||
                x.NormalizedEmail.Contains(normalizedSearch) ||
                x.FirstName.ToUpper().Contains(normalizedSearch) ||
                x.LastName.ToUpper().Contains(normalizedSearch));
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.UserName)
                .Skip(
                    (request.PageNumber - 1) *
                    request.PageSize)
                .Take(request.PageSize)
                .Select(x =>
                    new UserListItemResponse
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        FullName =
                            (x.FirstName + " " + x.LastName).Trim(),
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        OrganizationUnitId =
                            x.OrganizationUnitId,
                        OrganizationUnitCode =
                            x.OrganizationUnit != null
                                ? x.OrganizationUnit.Code
                                : null,
                        OrganizationUnitName =
                            x.OrganizationUnit != null
                                ? x.OrganizationUnit.Name
                                : null,
                        Roles =
                            x.UserRoles
                                .Select(ur => ur.Role.Code)
                                .OrderBy(code => code)
                                .ToArray(),
                        IsActive = x.IsActive,
                        IsLocked = x.IsLocked,
                        CreatedDate = x.CreatedDate,
                        Version = x.Version
                    })
                .ToListAsync(cancellationToken);

        return new PagedResult<UserListItemResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
