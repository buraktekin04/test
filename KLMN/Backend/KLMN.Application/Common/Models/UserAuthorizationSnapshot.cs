namespace KLMN.Application.Common.Models;

/// <summary>
/// Kullanıcının güncel rol ve efektif permission sonucunu temsil eder.
/// </summary>
public sealed record UserAuthorizationSnapshot(
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions,
    bool IsAdmin);
