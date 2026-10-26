using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.HcUnusedReasons.Responses;
using KLMN.Application.HcUnusedReasons.Commands.CreateHcUnusedReasonForExam;
using KLMN.Application.HcUnusedReasons.Commands.UpdateHcUnusedReasonForExam;
using KLMN.Application.HcUnusedReasons.Commands.ChangeStatusHcUnusedReasonForExam;
using KLMN.Application.HcUnusedReasons.Commands.DeleteHcUnusedReasonForExam;
using KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExams;
using KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExamById;
using KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExamOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Kullanılmayacak Muayene Sebepleri için bağımsız CRUD, options, audit ve Excel uçları.</summary>
[ApiController]
[Route("api/hc-unused-reasons")]
public sealed class HcUnusedReasonsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet][HasPermission(PermissionCodes.HcUnusedReasons.View, PermissionCodes.HcUnusedReasons.Query, PermissionCodes.HcUnusedReasons.Create, PermissionCodes.HcUnusedReasons.Update, PermissionCodes.HcUnusedReasons.Delete)]
 public async Task<IActionResult> List([FromQuery] GetHcUnusedReasonForExamsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("options")][HasPermission(PermissionCodes.HcUnusedReasons.View, PermissionCodes.HcUnusedReasons.Query, PermissionCodes.HcUnusedReasons.Create, PermissionCodes.HcUnusedReasons.Update, PermissionCodes.HcUnusedReasons.Delete)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedDecisionId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
     Ok(await sender.Send(new GetHcUnusedReasonForExamOptionsQuery(selectedDecisionId,selectedReferenceId),ct));
 [HttpGet("export")][HasPermission(PermissionCodes.HcUnusedReasons.Query)]
 public async Task<IActionResult> Export([FromQuery] GetHcUnusedReasonForExamsQuery q,CancellationToken ct)
 {
    var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
    if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="En fazla 5000 kayıt aktarılabilir."});
    var data=new List<HcUnusedReasonForExamResponse>(first.Items);
    for(var p=2;p<=(int)Math.Ceiling(first.TotalCount/100d);p++)data.AddRange((await sender.Send(q with{PageNumber=p,PageSize=100},ct)).Items);
    var cols=new ExcelColumn<HcUnusedReasonForExamResponse>[]{new("Kurul Kararı",x=>x.DecisionName,Width:42),new("İlişkili Tanım",x=>x.ReferenceName,Width:42),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
    return File(excel.Export(data,cols,"Kullanılmayacak Muayene Sebepleri"),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","hc-unused-reasons_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")][HasPermission(PermissionCodes.HcUnusedReasons.View, PermissionCodes.HcUnusedReasons.Query, PermissionCodes.HcUnusedReasons.Create, PermissionCodes.HcUnusedReasons.Update, PermissionCodes.HcUnusedReasons.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetHcUnusedReasonForExamByIdQuery(id),ct));
 [HttpPost][HasPermission(PermissionCodes.HcUnusedReasons.Create)]
 public async Task<IActionResult> Create([FromBody] CreateHcUnusedReasonForExamCommand cmd,CancellationToken ct)
 {var result=await sender.Send(cmd,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);}
 [HttpPut("{id:guid}")][HasPermission(PermissionCodes.HcUnusedReasons.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateHcUnusedReasonForExamCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpPatch("{id:guid}/status")][HasPermission(PermissionCodes.HcUnusedReasons.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusHcUnusedReasonForExamCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpDelete("{id:guid}")][HasPermission(PermissionCodes.HcUnusedReasons.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {await sender.Send(new DeleteHcUnusedReasonForExamCommand(id,version),ct);return NoContent();}
}
