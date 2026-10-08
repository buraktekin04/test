using KLMN.Application.Authentication.Responses;
using MediatR;

namespace KLMN.Application.Authentication.Queries.GetCurrentUser;

/// <summary>Authenticated kullanıcının güncel profil ve authorization bilgisini sorgular.</summary>
public sealed record GetCurrentUserQuery : IRequest<CurrentUserResponse>;
