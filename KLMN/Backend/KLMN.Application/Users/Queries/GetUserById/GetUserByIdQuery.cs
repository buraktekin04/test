using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Queries.GetUserById;

/// <summary>
/// Düzenleme ekranında kullanılmak üzere kullanıcıyı güncel
/// veritabanı kayıtlarından benzersiz ID ile getirir.
/// </summary>
/// <param name="Id">Sorgulanacak kullanıcı GUID değeridir.</param>
public sealed record GetUserByIdQuery(Guid Id)
    : IRequest<UserListItemResponse>;
