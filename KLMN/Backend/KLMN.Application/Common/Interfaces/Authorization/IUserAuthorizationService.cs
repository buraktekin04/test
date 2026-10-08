using KLMN.Application.Common.Authorization;

namespace KLMN.Application.Common.Interfaces.Authorization;

/// <summary>Kullanıcının rol ve effective permission bilgilerini hesaplar.</summary>
public interface IUserAuthorizationService
{
    /// <summary>
    /// Kullanıcının geçerli rol ve izin bilgisini asenkron olarak hesaplar.
    /// </summary>
    Task<UserAuthorizationSnapshot> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
