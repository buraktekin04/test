using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using KLMN.Domain.Entities.Identity;
using MediatR;
using KLMN.Application.Users.Commands.Common;
using KLMN.Application.Common.Interfaces.Authorization;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Commands.CreateUser;

/// <summary>
/// Benzersiz kullanıcı oluşturur; ilk parolayı hash'ler ve
/// seçilen rolleri veya seçim gönderilmezse STANDARD_USER rolünü atar.
/// </summary>
public sealed class CreateUserCommandHandler(
    IKLMNDbContext dbContext,
    IPasswordHasherService passwordHasherService,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService,
    IPermissionChecker permissionChecker,
    IUserAuthorizationService authorizationService)
    : IRequestHandler<CreateUserCommand, UserMutationResponse>
{
    /// <summary>İlk kullanıcıyı yalnızca gerekli identity bilgileriyle oluşturur.</summary>
    public async Task<UserMutationResponse> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Benzersizlik karşılaştırmalarında kullanılan kültürden bağımsız değerlerdir.
        var normalizedUserName = request.UserName.Trim().ToUpperInvariant();
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        // Pasif fakat silinmemiş hesaplar da benzersizlik kontrolüne dahildir.
        var exists = await dbContext.Users
            .IgnoreQueryFilters()
            .AnyAsync(x => !x.IsDeleted &&
                (x.NormalizedUserName == normalizedUserName ||
                 x.NormalizedEmail == normalizedEmail), cancellationToken);

        if (exists)
        {
            throw new UserConflictException("Kullanıcı adı veya e-posta zaten kullanılmaktadır.");
        }

        // Yeni kullanıcıya görev yeri seçildiyse, pasif veya silinmiş
        // birlik ataması API üzerinden de engellenir.
        if (request.MilitaryUnitDefId.HasValue)
        {
            var now = DateTime.UtcNow;
            var militaryUnitAvailable = await dbContext.MilitaryUnitDefs
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Id == request.MilitaryUnitDefId.Value &&
                               x.IsActive && !x.IsDeleted &&
                    (!x.ValidFrom.HasValue || x.ValidFrom <= now) &&
                    (!x.ValidTo.HasValue || now < x.ValidTo), cancellationToken);

            if (!militaryUnitAvailable)
            {
                throw new UserConflictException(
                    "Seçilen görev yeri bulunamadı, aktif değil veya geçerlilik dönemi dışında. Güncel birlik listesinden seçim yapınız.");
            }
        }

        // Domain entity; açık parola yerine Identity PasswordHasher çıktısı saklanır.
        var user = new User
        {
            UserName = request.UserName.Trim(),
            NormalizedUserName = normalizedUserName,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null : request.PhoneNumber.Trim(),
            PasswordHash = passwordHasherService.HashPassword(request.Password),
            ClockLimit = request.ClockLimit,
            MilitaryUnitDefId = request.MilitaryUnitDefId,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            PasswordChangedDate = timeProvider.GetUtcNow().UtcDateTime
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await UserRoleAssignment.ApplyAsync(dbContext, currentUserService, permissionChecker,
            authorizationService, user, request.RoleIds, true, cancellationToken);

        // SaveChanges interceptor'ı CreatedDate/CreatedBy alanlarını doldurur.
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserMutationResponse { Id = user.Id, Version = user.Version };
    }
}
