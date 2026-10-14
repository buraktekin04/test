using KLMN.Application.Users.Responses;
using MediatR;
namespace KLMN.Application.Users.Queries.GetUserRoleOptions;
public sealed record GetUserRoleOptionsQuery : IRequest<IReadOnlyList<UserRoleOptionResponse>>;
