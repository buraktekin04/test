namespace KLMN.Application.Common.Authorization;

/// <summary>Kullanıcının rol ve effective permission snapshot bilgisidir.</summary>
public sealed record UserAuthorizationSnapshot
{
    public required IReadOnlyCollection<string> Roles { get; init; }
    public required IReadOnlyCollection<string> Permissions { get; init; }
    public required bool IsAdmin { get; init; }
}
