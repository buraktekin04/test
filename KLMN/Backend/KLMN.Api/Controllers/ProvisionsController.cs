using KLMN.Api.Authorization;
using KLMN.Application.Provisions.Commands.CreateWithDispatch;
using KLMN.Application.Provisions.Commands.CreateWithoutDispatch;
using KLMN.Application.Provisions.Commands.CancelProvision;
using KLMN.Application.Provisions.Queries.GetProvisions;
using KLMN.Application.Provisions.Queries.GetProvisionById;
using KLMN.Application.Provisions.Queries.GetProvisionStates;
using KLMN.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>Eski MSKBS Provizyon Sevkli/Sevksiz, Sorgu, Takip, İptal API.</summary>
[ApiController]
[Route("api/provisions")]
public sealed class ProvisionsController(ISender sender):ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionCodes.Provisions.View,PermissionCodes.Provisions.Query,PermissionCodes.Provisions.Track)]
    public async Task<IActionResult> List([FromQuery] GetProvisionsQuery query,CancellationToken ct)
        =>Ok(await sender.Send(query,ct));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.Provisions.View,PermissionCodes.Provisions.Query,PermissionCodes.Provisions.Track)]
    public async Task<IActionResult> Detail(Guid id,CancellationToken ct)
        =>Ok(await sender.Send(new GetProvisionByIdQuery(id),ct));

    [HttpGet("{id:guid}/states")]
    [HasPermission(PermissionCodes.Provisions.Track,PermissionCodes.Provisions.Query)]
    public async Task<IActionResult> States(Guid id,CancellationToken ct)
        =>Ok(await sender.Send(new GetProvisionStatesQuery(id),ct));

    [HttpPost("with-dispatch")]
    [HasPermission(PermissionCodes.Provisions.CreateWithDispatch)]
    public async Task<IActionResult> WithDispatch([FromBody] CreateWithDispatchCommand command,CancellationToken ct)
    {
        var result=await sender.Send(command,ct);
        return CreatedAtAction(nameof(Detail),new {id=result.Id},result);
    }

    [HttpPost("without-dispatch")]
    [RequestSizeLimit(3*1024*1024)]
    [HasPermission(PermissionCodes.Provisions.CreateWithoutDispatch)]
    public async Task<IActionResult> WithoutDispatch([FromBody] CreateWithoutDispatchCommand command,CancellationToken ct)
    {
        var result=await sender.Send(command,ct);
        return CreatedAtAction(nameof(Detail),new {id=result.Id},result);
    }

    [HttpPatch("{id:guid}/cancel")]
    [HasPermission(PermissionCodes.Provisions.Cancel)]
    public async Task<IActionResult> Cancel(Guid id,[FromBody] CancelProvisionCommand command,CancellationToken ct)
    {
        if(id!=command.Id)return BadRequest("Provizyon kimliği uyuşmuyor.");
        return Ok(await sender.Send(command,ct));
    }
}
