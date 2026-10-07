using KLMN.Application.Common.Models;

namespace KLMN.Application.Common.Interfaces.Authorization;

/// <summary>
/// Kullanıcının rol ve efektif permission setini hesaplar.
/// </summary>
public interface IUserAuthorizationService
{
    Task<UserAuthorizationSnapshot> GetSnapshotAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
