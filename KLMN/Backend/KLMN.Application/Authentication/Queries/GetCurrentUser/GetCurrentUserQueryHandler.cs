using KLMN.Application.Authentication.Responses;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Queries.GetCurrentUser;

/// <summary>Mevcut kullanıcı bilgilerini ve güncel yetkilerini DB'den yükler.</summary>
public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    /// <summary>
    /// EF Core üzerinden veriye erişimi sağlayan Application katmanı veritabanı sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// İstek yapan kullanıcının kimliği ve istemci bilgilerine erişir.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;
    /// <summary>
    /// Rol izinlerini ve kullanıcı override'larını birleştirerek geçerli yetkileri hesaplar.
    /// </summary>
    private readonly IUserAuthorizationService _authorizationService;

    /// <summary>
    /// get current user query handler işlemini uygulama kurallarına göre gerçekleştirir.
    /// </summary>
    public GetCurrentUserQueryHandler(
        IKLMNDbContext dbContext,
        ICurrentUserService currentUserService,
        IUserAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// MediatR komutunu güvenlik ve iş kurallarına göre yürütür.
    /// </summary>
    public async Task<CurrentUserResponse> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        // Güncel oturumdan elde edilen kullanıcının benzersiz kimliğidir.
        var userId = _currentUserService.UserId
            ?? throw new AuthenticationRequiredException();

        // İlgili işlemin sahibi ve güvenlik durumu kontrol edilen kullanıcıdır.
        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new
            {
                x.Id,
                x.UserName,
                x.Email,
                x.FirstName,
                x.LastName,
                x.PhoneNumber,
                x.OrganizationUnitId,
                OrganizationUnitName = x.OrganizationUnit != null
                    ? x.OrganizationUnit.Name
                    : null,
                x.LastLoginDate,
                x.PasswordChangedDate
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new AuthenticationRequiredException();

        // Kullanıcının o anki rol ve effective permission bilgileridir.
        var authorization = await _authorizationService.GetAsync(
            user.Id,
            cancellationToken);

        return new CurrentUserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            PhoneNumber = user.PhoneNumber,
            OrganizationUnitId = user.OrganizationUnitId,
            OrganizationUnitName = user.OrganizationUnitName,
            LastLoginDate = user.LastLoginDate,
            PasswordChangedDate = user.PasswordChangedDate,
            Roles = authorization.Roles,
            Permissions = authorization.Permissions
        };
    }
}
