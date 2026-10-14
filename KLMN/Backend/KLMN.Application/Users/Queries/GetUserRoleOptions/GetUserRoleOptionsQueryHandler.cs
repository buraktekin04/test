using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Users.Queries.GetUserRoleOptions;
/// <summary>ADMIN hiçbir kullanıcı için atanabilir rol olarak sunulmaz.</summary>
public sealed class GetUserRoleOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetUserRoleOptionsQuery, IReadOnlyList<UserRoleOptionResponse>>
{
    public async Task<IReadOnlyList<UserRoleOptionResponse>> Handle(GetUserRoleOptionsQuery request, CancellationToken ct)
    {
        return await db.Roles.AsNoTracking()
            .Where(r => r.Code != SystemRoles.Admin)
            .OrderBy(r => r.Name)
            .Select(r => new UserRoleOptionResponse(r.Id, r.Code, r.Name))
            .ToListAsync(ct);
    }
}
