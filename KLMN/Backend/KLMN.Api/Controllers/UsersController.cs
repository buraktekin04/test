using KLMN.Application.Users.Queries.GetUsers;
using KLMN.Domain.Constants;
using KLMN.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>
/// Kullanıcı yönetimi endpointlerini sağlar.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController(
    ISender sender)
    : ControllerBase
{
    /// <summary>
    /// Kullanıcıları filtrelenmiş ve sayfalanmış şekilde listeler.
    /// </summary>
    [HttpGet]
    [HasPermission(
        PermissionCodes.Users.Query)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        var response =
            await sender.Send(
                query,
                cancellationToken);

        return Ok(response);
    }
}
