namespace KLMN.Application.Common.Interfaces.Identity;

/// <summary>
/// Mevcut authentication context'i üzerinden kullanıcı ve istemci bilgilerini sağlar.
/// Doğrudan veritabanına erişmez.
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
    bool IsAuthenticated { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
}
