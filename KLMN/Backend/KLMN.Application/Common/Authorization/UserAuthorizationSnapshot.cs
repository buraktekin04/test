namespace KLMN.Application.Common.Authorization;

/// <summary>Kullanıcının rol ve effective permission snapshot bilgisidir.</summary>
public sealed record UserAuthorizationSnapshot
{
    /// <summary>
    /// Kullanıcının etkin rol kodlarını içerir.
    /// </summary>
    public required IReadOnlyCollection<string> Roles { get; init; }
    /// <summary>
    /// Kullanıcının rol ve override kaynaklı etkin yetkilerini içerir.
    /// </summary>
    public required IReadOnlyCollection<string> Permissions { get; init; }
    /// <summary>
    /// ADMIN sistem rolünün etkin olup olmadığını belirtir.
    /// </summary>
    public required bool IsAdmin { get; init; }
}
