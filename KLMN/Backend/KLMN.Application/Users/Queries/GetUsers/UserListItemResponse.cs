namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>
/// Kullanıcı listeleme ekranında döndürülen özet kullanıcı modelidir.
/// </summary>
public sealed class UserListItemResponse
{
    public Guid Id { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? PhoneNumber { get; init; }

    public Guid? OrganizationUnitId { get; init; }

    public string? OrganizationUnitCode { get; init; }

    public string? OrganizationUnitName { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];

    public bool IsActive { get; init; }

    public bool IsLocked { get; init; }

    public DateTime CreatedDate { get; init; }

    public uint Version { get; init; }
}
