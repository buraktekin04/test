using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Commands.DeleteUser;

/// <summary>
/// Kullanıcıyı fiziksel silmek yerine SaveChanges interceptor üzerinden
/// soft delete yapar ve güvenli kullanıcı kontrollerini uygular.
/// </summary>
public sealed class DeleteUserCommandHandler(
    IKLMNDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeleteUserCommand>
{
    /// <summary>Versiyon, oturum sahibi ve ADMIN kontrollerinden sonra soft delete yapar.</summary>
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new UserNotFoundException();

        if (user.Version != request.Version)
            throw new DbUpdateConcurrencyException("Kullanıcı kaydı güncellenmiş. Listeyi yenileyiniz.");

        if (currentUserService.UserId == user.Id)
            throw new UserConflictException("Kendi hesabınızı silemezsiniz.");

        // Yönetim hesabının soft delete ile silinmesine izin verilmez.
        var isAdmin = await dbContext.UserRoles.AnyAsync(x =>
            x.UserId == user.Id && x.Role.Code == SystemRoles.Admin,
            cancellationToken);
        if (isAdmin)
            throw new UserConflictException("ADMIN rolündeki kullanıcı silinemez.");

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
