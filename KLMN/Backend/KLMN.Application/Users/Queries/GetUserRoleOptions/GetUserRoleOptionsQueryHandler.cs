using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Users.Queries.GetUserRoleOptions;
public sealed class GetUserRoleOptionsQueryHandler(IKLMNDbContext db,
    ICurrentUserService currentUser, IUserAuthorizationService authorization)
    : IRequestHandler<GetUserRoleOptionsQuery, IReadOnlyList<UserRoleOptionResponse>>
{
    public async Task<IReadOnlyList<UserRoleOptionResponse>> Handle(GetUserRoleOptionsQuery request, CancellationToken ct)
    {
        var actor = currentUser.UserId.HasValue ? await authorization.GetAsync(currentUser.UserId.Value, ct) : null;
        var admin = actor?.IsAdmin == true;
        return await db.Roles.AsNoTracking().Where(r => admin || r.Code != SystemRoles.Admin)
            .OrderBy(r => r.Name).Select(r => new UserRoleOptionResponse(r.Id, r.Code, r.Name)).ToListAsync(ct);
    }
}
