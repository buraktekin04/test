using KLMN.Api.Authorization;
using KLMN.Application.Users.Commands.ChangeUserStatus;
using KLMN.Application.Users.Commands.CreateUser;
using KLMN.Application.Users.Commands.DeleteUser;
using KLMN.Application.Users.Commands.UpdateUser;
using KLMN.Application.Users.Queries.GetUserById;
using KLMN.Application.Users.Queries.GetUsers;
using KLMN.Application.Users.Responses;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>
/// Kullanıcı tanımları ekranındaki listeleme, detay, oluşturma,
/// düzenleme, aktiflik ve soft delete işlemlerini sağlar.
/// </summary>
[ApiController]
[Route("api/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    /// <summary>Yetki kontrollü ve sayfalı kullanıcı listesi döndürür.</summary>
    [HttpGet]
    [HasPermission(PermissionCodes.Users.Query)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersQuery query, CancellationToken cancellationToken) =>
        Ok(await sender.Send(query, cancellationToken));

    /// <summary>Seçilen kullanıcıyı düzenleme ekranı için getirir.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Users.Query)]
    [ProducesResponseType(typeof(UserListItemResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserListItemResponse>> GetUserById(
        [FromRoute] Guid id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUserByIdQuery(id), cancellationToken));

    /// <summary>Parolası hash'lenmiş standart rol sahibi yeni kullanıcı oluşturur.</summary>
    [HttpPost]
    [HasPermission(PermissionCodes.Users.Create)]
    [ProducesResponseType(typeof(UserMutationResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserMutationResponse>> CreateUser(
        [FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    /// <summary>Kullanıcı profilini optimistic concurrency korumasıyla günceller.</summary>
    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.Users.Update)]
    [ProducesResponseType(typeof(UserMutationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserMutationResponse>> UpdateUser(
        [FromRoute] Guid id, [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route ID ve request ID eşleşmiyor.");
        return Ok(await sender.Send(command, cancellationToken));
    }

    /// <summary>Kullanıcı hesabını aktifleştirir veya pasifleştirir.</summary>
    [HttpPatch("{id:guid}/status")]
    [HasPermission(PermissionCodes.Users.Activate)]
    [ProducesResponseType(typeof(UserMutationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserMutationResponse>> ChangeStatus(
        [FromRoute] Guid id, [FromBody] ChangeUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route ID ve request ID eşleşmiyor.");
        return Ok(await sender.Send(command, cancellationToken));
    }

    /// <summary>Kullanıcı kaydını fiziksel silmeden soft delete yapar.</summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.Users.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] Guid id, [FromQuery] uint version,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteUserCommand(id, version), cancellationToken);
        return NoContent();
    }
}
