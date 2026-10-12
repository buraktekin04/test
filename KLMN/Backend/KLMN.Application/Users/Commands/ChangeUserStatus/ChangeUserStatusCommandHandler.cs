using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Commands.ChangeUserStatus;

/// <summary>
/// Kullanıcının aktifliğini değiştirir. Yönetici hesabının veya mevcut
/// oturumun yanlışlıkla pasifleştirilmesini önler.
/// </summary>
public sealed class ChangeUserStatusCommandHandler(
    IKLMNDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<ChangeUserStatusCommand, UserMutationResponse>
{
    /// <summary>Güncel versiyonu doğrulayarak istenen kullanıcı durumunu kaydeder.</summary>
    public async Task<UserMutationResponse> Handle(
        ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new UserNotFoundException();

        if (user.Version != request.Version)
            throw new DbUpdateConcurrencyException("Kullanıcı güncel versiyonla düzenlenmelidir.");

        if (!request.IsActive)
        {
            if (currentUserService.UserId == user.Id)
                throw new UserConflictException("Kendi hesabınızı pasifleştiremezsiniz.");

            // Sistem ADMIN hesabının özel kullanıcı komutuyla pasifleştirilmesi engellenir.
            var isAdmin = await dbContext.UserRoles.AnyAsync(x =>
                x.UserId == user.Id && x.Role.Code == SystemRoles.Admin,
                cancellationToken);

            if (isAdmin)
                throw new UserConflictException("ADMIN rolüne sahip kullanıcı pasifleştirilemez.");
        }

        user.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UserMutationResponse { Id = user.Id, Version = user.Version };
    }
}
