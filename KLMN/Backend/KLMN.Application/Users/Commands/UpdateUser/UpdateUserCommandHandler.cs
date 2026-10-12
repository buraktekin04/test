using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Exceptions;
using KLMN.Application.Users.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Commands.UpdateUser;

/// <summary>
/// Profil değişikliklerinde kullanıcı/email benzersizliğini ve
/// PostgreSQL xmin optimistic concurrency kontrolünü uygular.
/// </summary>
public sealed class UpdateUserCommandHandler(IKLMNDbContext dbContext)
    : IRequestHandler<UpdateUserCommand, UserMutationResponse>
{
    /// <summary>Mevcut versiyonu doğrular; birden çok kullanıcının çakışmasını 409'a dönüştürür.</summary>
    public async Task<UserMutationResponse> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Pasif kayıtlar düzenlenebilir, soft delete edilmiş kayıtlar düzenlenemez.
        var user = await dbContext.Users.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new UserNotFoundException();

        // İstemcinin açtığı kayıttan sonra veri değiştiyse üzerine yazılmaz.
        if (user.Version != request.Version)
        {
            throw new DbUpdateConcurrencyException("Kullanıcı kaydı başka bir işlemde değiştirilmiştir.");
        }

        var userName = request.UserName.Trim();
        var email = request.Email.Trim();
        var normalizedUserName = userName.ToUpperInvariant();
        var normalizedEmail = email.ToUpperInvariant();

        // Aynı kullanıcı hariç tutulur; aktif/pasif kayıtların benzersizliği korunur.
        var duplicate = await dbContext.Users.IgnoreQueryFilters()
            .AnyAsync(x => x.Id != user.Id && !x.IsDeleted &&
                (x.NormalizedUserName == normalizedUserName ||
                 x.NormalizedEmail == normalizedEmail), cancellationToken);

        if (duplicate)
        {
            throw new UserConflictException("Kullanıcı adı veya e-posta zaten kullanılmaktadır.");
        }

        /*
         * Birim değiştiriliyorsa hedef organizasyon aktif ve silinmemiş olmalı.
         * Daha önceden atanan birim sonradan pasifleştirildiyse, başka profil
         * alanlarının değiştirilmesine izin verilir; eski FK korunabilir.
         * Yeni atamalar için pasif birim kabul edilmez.
         */
        if (request.OrganizationUnitId.HasValue &&
            request.OrganizationUnitId != user.OrganizationUnitId)
        {
            var organizationAvailable = await dbContext.OrganizationUnits
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Id == request.OrganizationUnitId.Value &&
                               x.IsActive && !x.IsDeleted, cancellationToken);

            if (!organizationAvailable)
            {
                throw new UserConflictException(
                    "Seçilen görev yeri bulunamadı veya aktif değil. Güncel organizasyon listesinden seçim yapınız.");
            }
        }

        user.OrganizationUnitId = request.OrganizationUnitId;
        user.UserName = userName;
        user.NormalizedUserName = normalizedUserName;
        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.ClockLimit = request.ClockLimit;
        user.ValidFrom = request.ValidFrom;
        user.ValidTo = request.ValidTo;
        user.Email = email;
        user.NormalizedEmail = normalizedEmail;
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null : request.PhoneNumber.Trim();

        // SaveChangesInterceptor UpdatedDate ve UpdatedBy bilgisini doldurur.
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UserMutationResponse { Id = user.Id, Version = user.Version };
    }
}
