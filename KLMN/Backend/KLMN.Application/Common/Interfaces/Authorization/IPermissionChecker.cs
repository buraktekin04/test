namespace KLMN.Application.Common.Interfaces.Authorization;

/// <summary>Mevcut kullanıcının permission durumunu DB üzerinden kontrol eder.</summary>
public interface IPermissionChecker
{
    /// <summary>
    /// ADMIN, kullanıcı override ve rol izinlerini sırasıyla değerlendirerek erişimi belirler.
    /// </summary>
    Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default);
}
