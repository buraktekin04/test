using KLMN.Application.Common.Authorization;

namespace KLMN.Application.Common.Interfaces.Authorization;

/// <summary>Kullanıcının rol ve effective permission bilgilerini hesaplar.</summary>
public interface IUserAuthorizationService
{
    Task<UserAuthorizationSnapshot> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
