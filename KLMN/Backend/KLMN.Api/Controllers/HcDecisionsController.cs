using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.HcDecisions.Responses;
using KLMN.Application.HcDecisions.Commands.CreateHcDecisionDef;
using KLMN.Application.HcDecisions.Commands.UpdateHcDecisionDef;
using KLMN.Application.HcDecisions.Commands.ChangeStatusHcDecisionDef;
using KLMN.Application.HcDecisions.Commands.DeleteHcDecisionDef;
using KLMN.Application.HcDecisions.Queries.GetHcDecisionDefs;
using KLMN.Application.HcDecisions.Queries.GetHcDecisionDefById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Kurul Karar Tanımları için bağımsız CRUD, Excel ve yetki denetimi.</summary>
[ApiController]
[Route("api/hc-decisions")]
public sealed class HcDecisionsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet][HasPermission(PermissionCodes.HcDecisions.View, PermissionCodes.HcDecisions.Query, PermissionCodes.HcDecisions.Create, PermissionCodes.HcDecisions.Update, PermissionCodes.HcDecisions.Delete)]
 public async Task<IActionResult> List([FromQuery] GetHcDecisionDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("export")][HasPermission(PermissionCodes.HcDecisions.Query)]
 public async Task<IActionResult> Export([FromQuery] GetHcDecisionDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="En fazla 5000 kayıt aktarılabilir."});
   var data=new List<HcDecisionDefResponse>(first.Items);
   for(var p=2;p<=(int)Math.Ceiling(first.TotalCount/100d);p++)data.AddRange((await sender.Send(q with{PageNumber=p,PageSize=100},ct)).Items);
   var cols=new ExcelColumn<HcDecisionDefResponse>[] {new("Kod",x=>x.Code,Width:14),new("Karar Adı",x=>x.Name,Width:54),new("TimeInformationRequired",x=>x.TimeInformationRequired?"Evet":"Hayır"),new("AdditionalDescriptionRequired",x=>x.AdditionalDescriptionRequired?"Evet":"Hayır"),new("NegativeDecision",x=>x.NegativeDecision?"Evet":"Hayır"),new("ApprovalRequired",x=>x.ApprovalRequired?"Evet":"Hayır"),new("PertemApprovalRequired",x=>x.PertemApprovalRequired?"Evet":"Hayır"),new("DiagnosisRequired",x=>x.DiagnosisRequired?"Evet":"Hayır"),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(data,cols,"Kurul Karar Tanımları"),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","hc-decisions_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")][HasPermission(PermissionCodes.HcDecisions.View, PermissionCodes.HcDecisions.Query, PermissionCodes.HcDecisions.Create, PermissionCodes.HcDecisions.Update, PermissionCodes.HcDecisions.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetHcDecisionDefByIdQuery(id),ct));
 [HttpPost][HasPermission(PermissionCodes.HcDecisions.Create)]
 public async Task<IActionResult> Create([FromBody] CreateHcDecisionDefCommand cmd,CancellationToken ct)
 {var result=await sender.Send(cmd,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);}
 [HttpPut("{id:guid}")][HasPermission(PermissionCodes.HcDecisions.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateHcDecisionDefCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpPatch("{id:guid}/status")][HasPermission(PermissionCodes.HcDecisions.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusHcDecisionDefCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpDelete("{id:guid}")][HasPermission(PermissionCodes.HcDecisions.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {await sender.Send(new DeleteHcDecisionDefCommand(id,version),ct);return NoContent();}
}
