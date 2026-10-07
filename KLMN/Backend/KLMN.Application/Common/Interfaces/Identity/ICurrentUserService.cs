namespace KLMN.Application.Common.Interfaces.Identity;

/// <summary>
/// Mevcut HTTP request içerisindeki authenticated kullanıcı bilgilerini sağlar.
/// Bu servis DB'ye gitmez; request claim/context bilgisini okur.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? UserName { get; }

    string? Email { get; }

    string? FirstName { get; }

    string? LastName { get; }

    string? FullName { get; }

    Guid? OrganizationUnitId { get; }

    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<string> Permissions { get; }

    string? SecurityStamp { get; }

    bool IsAuthenticated { get; }

    string? IpAddress { get; }

    string? UserAgent { get; }
}
