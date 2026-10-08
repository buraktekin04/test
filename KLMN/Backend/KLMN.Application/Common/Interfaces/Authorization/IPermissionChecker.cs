namespace KLMN.Application.Common.Interfaces.Authorization;

/// <summary>Mevcut kullanıcının permission durumunu DB üzerinden kontrol eder.</summary>
public interface IPermissionChecker
{
    Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default);
}
