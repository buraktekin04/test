using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Commands.UpdateUser;

/// <summary>
/// Kullanıcı profilini eşzamanlılık versiyonu ile güncelleme isteğidir.
/// Parola, rol ve özel yetkiler bu komutla değiştirilemez.
/// </summary>
public sealed record UpdateUserCommand : IRequest<UserMutationResponse>
{
    /// <summary>Güncellenecek kullanıcının benzersiz kimliğidir.</summary>
    public Guid Id { get; init; }
    /// <summary>İstemcinin son okuduğu PostgreSQL xmin değeridir.</summary>
    public uint Version { get; init; }
    /// <summary>Benzersiz giriş kullanıcı adıdır.</summary>
    public required string UserName { get; init; }
    /// <summary>Güncellenecek addır.</summary>
    public required string FirstName { get; init; }
    /// <summary>Güncellenecek soyaddır.</summary>
    public required string LastName { get; init; }
    /// <summary>Benzersiz e-posta adresidir.</summary>
    public required string Email { get; init; }
    /// <summary>İsteğe bağlı telefon numarasıdır.</summary>
    public string? PhoneNumber { get; init; }
}
