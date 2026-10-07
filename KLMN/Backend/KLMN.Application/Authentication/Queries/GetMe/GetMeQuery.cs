using KLMN.Application.Authentication.Models;
using MediatR;

namespace KLMN.Application.Authentication.Queries.GetMe;

/// <summary>
/// Authenticated kullanıcının güncel profil ve authorization bilgisini getirir.
/// </summary>
public sealed record GetMeQuery
    : IRequest<AuthUserResponse>;
