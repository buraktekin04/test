namespace KLMN.Application.Authentication.Models;

/// <summary>
/// Authentication response içerisinde döndürülen kullanıcı bilgisidir.
/// </summary>
public sealed class AuthUserResponse
{
    public Guid Id { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public Guid? OrganizationUnitId { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];

    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
