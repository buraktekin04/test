using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Application.Users.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Queries.GetUserById;

/// <summary>Silinmemiş aktif/pasif kullanıcının detay bilgilerini getirir.</summary>
public sealed class GetUserByIdQueryHandler(IKLMNDbContext dbContext)
    : IRequestHandler<GetUserByIdQuery, UserListItemResponse>
{
    /// <summary>
    /// Kullanıcı bilgilerini takip etmeyen sorguyla okur. Inactive
    /// kullanıcı düzenlemesi desteklenirken soft-delete gizli tutulur.
    /// </summary>
    public async Task<UserListItemResponse> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        // İstemciye döndürülecek, parola ve token içermeyen kullanıcı DTO'sudur.
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
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
                    ? x.OrganizationUnit.Code : null,
                OrganizationUnitName = x.OrganizationUnit != null
                    ? x.OrganizationUnit.Name : null,
                Roles = x.UserRoles
                    .Where(ur => ur.IsActive && !ur.IsDeleted
                        && ur.Role.IsActive && !ur.Role.IsDeleted)
                    .Select(ur => ur.Role.Code)
                    .OrderBy(code => code)
                    .ToArray(),
                IsActive = x.IsActive,
                IsLocked = x.IsLocked,
                ClockLimit = x.ClockLimit,
                CreatedDate = x.CreatedDate,
                Version = x.Version
            })
            .SingleOrDefaultAsync(cancellationToken);

        return user ?? throw new UserNotFoundException();
    }
}
