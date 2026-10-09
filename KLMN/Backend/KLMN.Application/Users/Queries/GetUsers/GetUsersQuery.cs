using KLMN.Application.Common.Models;
using MediatR;

using KLMN.Application.Users.Responses;

namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>
/// Kullanıcıları filtrelenmiş ve sayfalanmış olarak getirir.
/// </summary>
public sealed record GetUsersQuery(
    string? Search = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<PagedResult<UserListItemResponse>>;
