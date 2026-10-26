using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.HcUnusedDecisions.Responses;
using KLMN.Application.HcUnusedDecisions.Commands.CreateHcUnusedDecision;
using KLMN.Application.HcUnusedDecisions.Commands.UpdateHcUnusedDecision;
using KLMN.Application.HcUnusedDecisions.Commands.ChangeStatusHcUnusedDecision;
using KLMN.Application.HcUnusedDecisions.Commands.DeleteHcUnusedDecision;
using KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisions;
using KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisionById;
using KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisionOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Birlikte Verilmeyecek Kararlar için bağımsız CRUD, options, audit ve Excel uçları.</summary>
[ApiController]
[Route("api/hc-unused-decisions")]
public sealed class HcUnusedDecisionsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet][HasPermission(PermissionCodes.HcUnusedDecisions.View, PermissionCodes.HcUnusedDecisions.Query, PermissionCodes.HcUnusedDecisions.Create, PermissionCodes.HcUnusedDecisions.Update, PermissionCodes.HcUnusedDecisions.Delete)]
 public async Task<IActionResult> List([FromQuery] GetHcUnusedDecisionsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("options")][HasPermission(PermissionCodes.HcUnusedDecisions.View, PermissionCodes.HcUnusedDecisions.Query, PermissionCodes.HcUnusedDecisions.Create, PermissionCodes.HcUnusedDecisions.Update, PermissionCodes.HcUnusedDecisions.Delete)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedDecisionId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
     Ok(await sender.Send(new GetHcUnusedDecisionOptionsQuery(selectedDecisionId,selectedReferenceId),ct));
 [HttpGet("export")][HasPermission(PermissionCodes.HcUnusedDecisions.Query)]
 public async Task<IActionResult> Export([FromQuery] GetHcUnusedDecisionsQuery q,CancellationToken ct)
 {
    var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
    if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="En fazla 5000 kayıt aktarılabilir."});
    var data=new List<HcUnusedDecisionResponse>(first.Items);
    for(var p=2;p<=(int)Math.Ceiling(first.TotalCount/100d);p++)data.AddRange((await sender.Send(q with{PageNumber=p,PageSize=100},ct)).Items);
    var cols=new ExcelColumn<HcUnusedDecisionResponse>[]{new("Kurul Kararı",x=>x.DecisionName,Width:42),new("İlişkili Tanım",x=>x.ReferenceName,Width:42),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
    return File(excel.Export(data,cols,"Birlikte Verilmeyecek Kararlar"),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","hc-unused-decisions_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")][HasPermission(PermissionCodes.HcUnusedDecisions.View, PermissionCodes.HcUnusedDecisions.Query, PermissionCodes.HcUnusedDecisions.Create, PermissionCodes.HcUnusedDecisions.Update, PermissionCodes.HcUnusedDecisions.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetHcUnusedDecisionByIdQuery(id),ct));
 [HttpPost][HasPermission(PermissionCodes.HcUnusedDecisions.Create)]
 public async Task<IActionResult> Create([FromBody] CreateHcUnusedDecisionCommand cmd,CancellationToken ct)
 {var result=await sender.Send(cmd,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);}
 [HttpPut("{id:guid}")][HasPermission(PermissionCodes.HcUnusedDecisions.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateHcUnusedDecisionCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpPatch("{id:guid}/status")][HasPermission(PermissionCodes.HcUnusedDecisions.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusHcUnusedDecisionCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpDelete("{id:guid}")][HasPermission(PermissionCodes.HcUnusedDecisions.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {await sender.Send(new DeleteHcUnusedDecisionCommand(id,version),ct);return NoContent();}
}
