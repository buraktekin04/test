using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Commands.CreateUser;

/// <summary>Parola hash'lenerek standart kullanıcı oluşturma isteğidir.</summary>
public sealed record CreateUserCommand : IRequest<UserMutationResponse>
{
    /// <summary>Kullanıcının benzersiz giriş adıdır.</summary>
    public required string UserName { get; init; }
    /// <summary>Kullanıcının adıdır.</summary>
    public required string FirstName { get; init; }
    /// <summary>Kullanıcının soyadıdır.</summary>
    public required string LastName { get; init; }
    /// <summary>Benzersiz e-posta adresidir.</summary>
    public required string Email { get; init; }
    /// <summary>İsteğe bağlı telefon numarasıdır.</summary>
    public string? PhoneNumber { get; init; }
    /// <summary>Yalnızca bu istekte alınan, veritabanına açık yazılmayan ilk paroladır.</summary>
    public required string Password { get; init; }

    /// <summary>
    /// true olduğunda kullanıcı sadece tanımlanan mesai gün/saatlerinde giriş yapabilir;
    /// false olduğunda mesai dışı erişimi yöneticisi tarafından onaylanmış kabul edilir.
    /// </summary>
    public bool ClockLimit { get; init; } = true;
}
