using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.HcDecisionConnections.Responses;
using KLMN.Application.HcDecisionConnections.Commands.CreateHcConnectedDecision;
using KLMN.Application.HcDecisionConnections.Commands.UpdateHcConnectedDecision;
using KLMN.Application.HcDecisionConnections.Commands.ChangeStatusHcConnectedDecision;
using KLMN.Application.HcDecisionConnections.Commands.DeleteHcConnectedDecision;
using KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisions;
using KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisionById;
using KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisionOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Bağlı Kararlar için bağımsız CRUD, options, audit ve Excel uçları.</summary>
[ApiController]
[Route("api/hc-decision-connections")]
public sealed class HcDecisionConnectionsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet][HasPermission(PermissionCodes.HcDecisionConnections.View, PermissionCodes.HcDecisionConnections.Query, PermissionCodes.HcDecisionConnections.Create, PermissionCodes.HcDecisionConnections.Update, PermissionCodes.HcDecisionConnections.Delete)]
 public async Task<IActionResult> List([FromQuery] GetHcConnectedDecisionsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("options")][HasPermission(PermissionCodes.HcDecisionConnections.View, PermissionCodes.HcDecisionConnections.Query, PermissionCodes.HcDecisionConnections.Create, PermissionCodes.HcDecisionConnections.Update, PermissionCodes.HcDecisionConnections.Delete)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedDecisionId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
     Ok(await sender.Send(new GetHcConnectedDecisionOptionsQuery(selectedDecisionId,selectedReferenceId),ct));
 [HttpGet("export")][HasPermission(PermissionCodes.HcDecisionConnections.Query)]
 public async Task<IActionResult> Export([FromQuery] GetHcConnectedDecisionsQuery q,CancellationToken ct)
 {
    var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
    if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="En fazla 5000 kayıt aktarılabilir."});
    var data=new List<HcConnectedDecisionResponse>(first.Items);
    for(var p=2;p<=(int)Math.Ceiling(first.TotalCount/100d);p++)data.AddRange((await sender.Send(q with{PageNumber=p,PageSize=100},ct)).Items);
    var cols=new ExcelColumn<HcConnectedDecisionResponse>[]{new("Kurul Kararı",x=>x.DecisionName,Width:42),new("İlişkili Tanım",x=>x.ReferenceName,Width:42),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
    return File(excel.Export(data,cols,"Bağlı Kararlar"),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","hc-decision-connections_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")][HasPermission(PermissionCodes.HcDecisionConnections.View, PermissionCodes.HcDecisionConnections.Query, PermissionCodes.HcDecisionConnections.Create, PermissionCodes.HcDecisionConnections.Update, PermissionCodes.HcDecisionConnections.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetHcConnectedDecisionByIdQuery(id),ct));
 [HttpPost][HasPermission(PermissionCodes.HcDecisionConnections.Create)]
 public async Task<IActionResult> Create([FromBody] CreateHcConnectedDecisionCommand cmd,CancellationToken ct)
 {var result=await sender.Send(cmd,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);}
 [HttpPut("{id:guid}")][HasPermission(PermissionCodes.HcDecisionConnections.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateHcConnectedDecisionCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpPatch("{id:guid}/status")][HasPermission(PermissionCodes.HcDecisionConnections.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusHcConnectedDecisionCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpDelete("{id:guid}")][HasPermission(PermissionCodes.HcDecisionConnections.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {await sender.Send(new DeleteHcConnectedDecisionCommand(id,version),ct);return NoContent();}
}
