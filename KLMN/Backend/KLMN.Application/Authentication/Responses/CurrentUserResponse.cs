namespace KLMN.Application.Authentication.Responses;

/// <summary>
/// Kimliği doğrulanmış kullanıcının güncel profil ve authorization bilgisidir.
/// </summary>
public sealed record CurrentUserResponse
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public Guid? OrganizationUnitId { get; init; }
    public string? OrganizationUnitName { get; init; }
    public DateTime? LastLoginDate { get; init; }
    public DateTime? PasswordChangedDate { get; init; }
    public required IReadOnlyCollection<string> Roles { get; init; }
    public required IReadOnlyCollection<string> Permissions { get; init; }
}
