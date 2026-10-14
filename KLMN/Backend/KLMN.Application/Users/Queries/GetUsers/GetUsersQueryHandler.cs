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

        // Kolon aramaları veritabanındaki tüm kayıtlar üzerinde uygulanır;
        // yalnızca istemcinin görebildiği 20 satırı filtrelemeyiz.
        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var name = request.FullName.Trim().ToUpperInvariant();
            query = query.Where(x =>
                (x.FirstName + " " + x.LastName).ToUpper().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            var userName = request.UserName.Trim().ToUpperInvariant();
            query = query.Where(x => x.NormalizedUserName.Contains(userName));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToUpperInvariant();
            query = query.Where(x => x.NormalizedEmail.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(request.MilitaryUnit))
        {
            var militaryUnit = request.MilitaryUnit.Trim().ToUpperInvariant();
            query = query.Where(x => x.MilitaryUnitDef != null &&
                (x.MilitaryUnitDef.Name.ToUpper().Contains(militaryUnit) ||
                 x.MilitaryUnitDef.Code.ToUpper().Contains(militaryUnit)));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var role = request.Role.Trim().ToUpperInvariant();
            query = query.Where(x => x.UserRoles.Any(ur =>
                ur.IsActive && !ur.IsDeleted &&
                ur.Role.IsActive && !ur.Role.IsDeleted &&
                ur.Role.Code.ToUpper().Contains(role)));
        }

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (request.IsLocked.HasValue)
            query = query.Where(x => x.IsLocked == request.IsLocked.Value);

        if (request.ClockLimit.HasValue)
            query = query.Where(x => x.ClockLimit == request.ClockLimit.Value);

        // PostgreSQL timestamptz değerlerinin UTC gün sınırlarına göre karşılaştırılması.
        if (request.ValidFromDate.HasValue)
        {
            var first = DateTime.SpecifyKind(
                request.ValidFromDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var next = first.AddDays(1);
            query = query.Where(x => x.ValidFrom >= first && x.ValidFrom < next);
        }

        if (request.ValidToDate.HasValue)
        {
            var first = DateTime.SpecifyKind(
                request.ValidToDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var next = first.AddDays(1);
            query = query.Where(x => x.ValidTo >= first && x.ValidTo < next);
        }

        if (request.CreatedDate.HasValue)
        {
            var first = DateTime.SpecifyKind(
                request.CreatedDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var next = first.AddDays(1);
            query = query.Where(x => x.CreatedDate >= first && x.CreatedDate < next);
        }

        // Filtreleme sonrası eşleşen kullanıcı sayısıdır.
        var totalCount = await query.CountAsync(cancellationToken);

        // Seçili sayfada döndürülecek kullanıcı DTO kayıtlarıdır.
        var items = await GetUsersSorting.Definition.Apply(query, request.SortField, request.SortOrder)
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
                MilitaryUnitDefId = x.MilitaryUnitDefId,
                MilitaryUnitDefCode = x.MilitaryUnitDef != null
                    ? x.MilitaryUnitDef.Code
                    : null,
                MilitaryUnitDefName = x.MilitaryUnitDef != null
                    ? x.MilitaryUnitDef.Name
                    : null,
                // Kayıtlar pasifler dahil sorgulandığında filtreler
                // kapalı olabileceğinden rol ilişkilerinde aktiflik ayrıca doğrulanır.
                Roles = x.UserRoles
                    .Where(ur => ur.IsActive && !ur.IsDeleted
                        && ur.Role.IsActive && !ur.Role.IsDeleted)
                    .Select(ur => ur.Role.Code)
                    .OrderBy(code => code)
                    .ToArray(),
                IsActive = x.IsActive,
                IsLocked = x.IsLocked,
                ClockLimit = x.ClockLimit,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
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
