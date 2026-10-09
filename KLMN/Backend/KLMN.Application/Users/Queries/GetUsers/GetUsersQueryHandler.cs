using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

using KLMN.Application.Users.Responses;

namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>Kullanıcı listeleme sorgusunu EF Core üzerinden çalıştırır.</summary>
internal sealed class GetUsersQueryHandler(
    IKLMNDbContext dbContext)
    : IRequestHandler<GetUsersQuery, PagedResult<UserListItemResponse>>
{
    /// <summary>
    /// Filtreleme, sayfalama ve izin kurallarına uygun kullanıcı listesini sorgular.
    /// </summary>
    public async Task<PagedResult<UserListItemResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            dbContext.Users.AsNoTracking();

        if (request.IncludeInactive)
        {
            // ActiveFilter dahil tüm filtreleri kapatıp soft-delete kuralını
            // manuel olarak koruyoruz. Böylece Application katmanı Persistence
            // filter-name sabitlerine bağımlı olmaz.
            query = query
                .IgnoreQueryFilters()
                .Where(x => !x.IsDeleted);
        }

        // Kullanıcının arama metninden temizlenmiş sorgu anahtarıdır.
        var search = request.Search?.Trim();

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Karşılaştırma için normalize edilmiş kullanıcı adı veya e-posta metnidir.
            var normalized = search.ToUpperInvariant();

            query = query.Where(x =>
                x.NormalizedUserName.Contains(normalized) ||
                x.NormalizedEmail.Contains(normalized) ||
                x.FirstName.ToUpper().Contains(normalized) ||
                x.LastName.ToUpper().Contains(normalized));
        }

        // Filtreleme sonrası eşleşen kullanıcı sayısıdır.
        var totalCount = await query.CountAsync(cancellationToken);

        // Seçili sayfada döndürülecek kullanıcı DTO kayıtlarıdır.
        var items = await query
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ThenBy(x => x.UserName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new UserListItemResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = (x.FirstName + " " + x.LastName).Trim(),
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                OrganizationUnitId = x.OrganizationUnitId,
                OrganizationUnitCode = x.OrganizationUnit != null
                    ? x.OrganizationUnit.Code
                    : null,
                OrganizationUnitName = x.OrganizationUnit != null
                    ? x.OrganizationUnit.Name
                    : null,
                Roles = x.UserRoles
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
