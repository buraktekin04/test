namespace KLMN.Application.Common.Interfaces.Authorization;

/// <summary>
/// Gerçek endpoint permission kararını DB üzerinden verir.
/// </summary>
public interface IPermissionChecker
{
    Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default);
}
