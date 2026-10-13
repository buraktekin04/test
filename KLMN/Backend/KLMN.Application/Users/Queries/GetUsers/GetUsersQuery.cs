using KLMN.Application.Common.Models;
using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Queries.GetUsers;

/// <summary>
/// Kullanıcı listesini sunucu tarafında arayan, kolon bazlı filtreleyen ve
/// sayfalayan CQRS sorgusudur. Aynı filtre sözleşmesi Excel aktarımında kullanılır.
/// </summary>
public sealed record GetUsersQuery(
    string? Search = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    string? FullName = null,
    string? UserName = null,
    string? Email = null,
    string? MilitaryUnit = null,
    string? Role = null,
    bool? IsActive = null,
    bool? IsLocked = null,
    bool? ClockLimit = null,
    DateOnly? ValidFromDate = null,
    DateOnly? ValidToDate = null,
    DateOnly? CreatedDate = null)
    : IRequest<PagedResult<UserListItemResponse>>;
