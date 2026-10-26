using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.HcUnusedAdmissionTypes.Responses;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.CreateHcUnusedAdmissionType;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.UpdateHcUnusedAdmissionType;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.ChangeStatusHcUnusedAdmissionType;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.DeleteHcUnusedAdmissionType;
using KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypes;
using KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypeById;
using KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypeOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Kullanılmayacak Başvuru Grupları için bağımsız CRUD, options, audit ve Excel uçları.</summary>
[ApiController]
[Route("api/hc-unused-admission-types")]
public sealed class HcUnusedAdmissionTypesController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.View, PermissionCodes.HcUnusedAdmissionTypes.Query, PermissionCodes.HcUnusedAdmissionTypes.Create, PermissionCodes.HcUnusedAdmissionTypes.Update, PermissionCodes.HcUnusedAdmissionTypes.Delete)]
 public async Task<IActionResult> List([FromQuery] GetHcUnusedAdmissionTypesQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("options")][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.View, PermissionCodes.HcUnusedAdmissionTypes.Query, PermissionCodes.HcUnusedAdmissionTypes.Create, PermissionCodes.HcUnusedAdmissionTypes.Update, PermissionCodes.HcUnusedAdmissionTypes.Delete)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedDecisionId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
     Ok(await sender.Send(new GetHcUnusedAdmissionTypeOptionsQuery(selectedDecisionId,selectedReferenceId),ct));
 [HttpGet("export")][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.Query)]
 public async Task<IActionResult> Export([FromQuery] GetHcUnusedAdmissionTypesQuery q,CancellationToken ct)
 {
    var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
    if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="En fazla 5000 kayıt aktarılabilir."});
    var data=new List<HcUnusedAdmissionTypeResponse>(first.Items);
    for(var p=2;p<=(int)Math.Ceiling(first.TotalCount/100d);p++)data.AddRange((await sender.Send(q with{PageNumber=p,PageSize=100},ct)).Items);
    var cols=new ExcelColumn<HcUnusedAdmissionTypeResponse>[]{new("Kurul Kararı",x=>x.DecisionName,Width:42),new("İlişkili Tanım",x=>x.ReferenceName,Width:42),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
    return File(excel.Export(data,cols,"Kullanılmayacak Başvuru Grupları"),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","hc-unused-admission-types_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.View, PermissionCodes.HcUnusedAdmissionTypes.Query, PermissionCodes.HcUnusedAdmissionTypes.Create, PermissionCodes.HcUnusedAdmissionTypes.Update, PermissionCodes.HcUnusedAdmissionTypes.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetHcUnusedAdmissionTypeByIdQuery(id),ct));
 [HttpPost][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.Create)]
 public async Task<IActionResult> Create([FromBody] CreateHcUnusedAdmissionTypeCommand cmd,CancellationToken ct)
 {var result=await sender.Send(cmd,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);}
 [HttpPut("{id:guid}")][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateHcUnusedAdmissionTypeCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpPatch("{id:guid}/status")][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusHcUnusedAdmissionTypeCommand cmd,CancellationToken ct)
 {if(id!=cmd.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");return Ok(await sender.Send(cmd,ct));}
 [HttpDelete("{id:guid}")][HasPermission(PermissionCodes.HcUnusedAdmissionTypes.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {await sender.Send(new DeleteHcUnusedAdmissionTypeCommand(id,version),ct);return NoContent();}
}
