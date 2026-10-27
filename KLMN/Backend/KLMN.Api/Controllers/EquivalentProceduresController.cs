using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.EquivalentProcedures.Responses;
using KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedures;
using KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedureOptions;
using KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedureById;
using KLMN.Application.EquivalentProcedures.Commands.CreateEquivalentProcedure;
using KLMN.Application.EquivalentProcedures.Commands.UpdateEquivalentProcedure;
using KLMN.Application.EquivalentProcedures.Commands.ChangeStatusEquivalentProcedure;
using KLMN.Application.EquivalentProcedures.Commands.DeleteEquivalentProcedure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>İki tetkik arasındaki eşdeğerlik CRUD, seçenek ve Excel uçları.</summary>
[ApiController][Route("api/equivalent-procedures")]
public sealed class EquivalentProceduresController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet][HasPermission(PermissionCodes.EquivalentProcedures.View, PermissionCodes.EquivalentProcedures.Query, PermissionCodes.EquivalentProcedures.Create, PermissionCodes.EquivalentProcedures.Update, PermissionCodes.EquivalentProcedures.Delete)]
 public async Task<IActionResult> List([FromQuery] GetEquivalentProceduresQuery q,CancellationToken ct)
  =>Ok(await sender.Send(q,ct));
 [HttpGet("options")][HasPermission(PermissionCodes.EquivalentProcedures.View, PermissionCodes.EquivalentProcedures.Query, PermissionCodes.EquivalentProcedures.Create, PermissionCodes.EquivalentProcedures.Update, PermissionCodes.EquivalentProcedures.Delete)]
 public async Task<IActionResult> Options([FromQuery] GetEquivalentProcedureOptionsQuery q,CancellationToken ct)
  =>Ok(await sender.Send(q,ct));
 [HttpGet("export")][HasPermission(PermissionCodes.EquivalentProcedures.Query)]
 public async Task<IActionResult> Export([FromQuery] GetEquivalentProceduresQuery q,CancellationToken ct)
 {
    var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
    if(first.TotalCount>5000)return BadRequest(new ProblemDetails{
      Status=400,Title="Excel sınırı aşıldı.",Detail="En fazla 5.000 eşleştirme aktarılabilir."});
    var list=new List<EquivalentProcedureResponse>(first.Items);
    for(int page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
      list.AddRange((await sender.Send(q with{PageNumber=page,PageSize=100},ct)).Items);
    ExcelColumn<EquivalentProcedureResponse>[] columns={
      new("Tetkik Kodu",x=>x.ProcedureCode,Width:20),
      new("Tetkik Adı",x=>x.ProcedureName,Width:40),
      new("Eşdeğer Tetkik Kodu",x=>x.EquivalentProcedureCode,Width:20),
      new("Eşdeğer Tetkik Adı",x=>x.EquivalentProcedureName,Width:40),
      new("Durum",x=>x.IsActive?"Aktif":"Pasif",Width:14)};
    return File(excel.Export(list,columns,"Eşdeğer Tetkikler"),
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "esdeger_tetkikler_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")][HasPermission(PermissionCodes.EquivalentProcedures.View, PermissionCodes.EquivalentProcedures.Query, PermissionCodes.EquivalentProcedures.Create, PermissionCodes.EquivalentProcedures.Update, PermissionCodes.EquivalentProcedures.Delete)]
 public async Task<IActionResult> Detail(Guid id,CancellationToken ct)
  =>Ok(await sender.Send(new GetEquivalentProcedureByIdQuery(id),ct));
 [HttpPost][HasPermission(PermissionCodes.EquivalentProcedures.Create)]
 public async Task<IActionResult> Create([FromBody] CreateEquivalentProcedureCommand r,CancellationToken ct){
    var value=await sender.Send(r,ct);return CreatedAtAction(nameof(Detail),new{id=value.Id},value);
 }
 [HttpPut("{id:guid}")][HasPermission(PermissionCodes.EquivalentProcedures.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateEquivalentProcedureCommand r,CancellationToken ct){
    if(id!=r.Id)return BadRequest("Kimlikler uyuşmuyor.");return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")][HasPermission(PermissionCodes.EquivalentProcedures.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusEquivalentProcedureCommand r,CancellationToken ct){
    if(id!=r.Id)return BadRequest("Kimlikler uyuşmuyor.");return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")][HasPermission(PermissionCodes.EquivalentProcedures.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct){
   await sender.Send(new DeleteEquivalentProcedureCommand(id,version),ct);return NoContent();
 }
}
