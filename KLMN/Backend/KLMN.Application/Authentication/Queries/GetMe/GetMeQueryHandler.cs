using KLMN.Application.Authentication.Models;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Queries.GetMe;

/// <summary>
/// Güncel authenticated kullanıcı bilgisini DB'den üretir.
/// </summary>
internal sealed class GetMeQueryHandler(
    IKLMNDbContext dbContext,
    ICurrentUserService currentUserService,
    IUserAuthorizationService userAuthorizationService)
    : IRequestHandler<GetMeQuery, AuthUserResponse>
{
    public async Task<AuthUserResponse> Handle(
        GetMeQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not Guid userId)
        {
            throw new AuthenticationRequiredException();
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken)
            ?? throw new AuthenticationRequiredException();

        var authorization =
            await userAuthorizationService.GetSnapshotAsync(
                user.Id,
                cancellationToken);

        return new AuthUserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Email = user.Email,
            OrganizationUnitId = user.OrganizationUnitId,
            Roles = authorization.Roles,
            Permissions = authorization.Permissions
        };
    }
}
