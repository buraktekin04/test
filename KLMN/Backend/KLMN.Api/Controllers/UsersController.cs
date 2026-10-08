using KLMN.Api.Authorization;
using KLMN.Application.Users.Queries.GetUsers;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Kullanıcı yönetimi endpointlerini sağlar.</summary>
[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Kullanıcıları filtrelenmiş ve sayfalanmış şekilde listeler.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Users.Query)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _sender.Send(
                query,
                cancellationToken));
    }
}
