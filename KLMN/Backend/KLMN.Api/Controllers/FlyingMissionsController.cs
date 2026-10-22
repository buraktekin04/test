using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.FlyingMissions.Responses;
using KLMN.Application.FlyingMissions.Commands.CreateFlyingMissionDef;
using KLMN.Application.FlyingMissions.Commands.UpdateFlyingMissionDef;
using KLMN.Application.FlyingMissions.Commands.ChangeStatusFlyingMissionDef;
using KLMN.Application.FlyingMissions.Commands.DeleteFlyingMissionDef;
using KLMN.Application.FlyingMissions.Queries.GetFlyingMissionDefs;
using KLMN.Application.FlyingMissions.Queries.GetFlyingMissionDefById;

using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Uçucu Görev Tanımları CRUD, Excel ve izin kontrollü API.</summary>
[ApiController]
[Route("api/flying-missions")]
public sealed class FlyingMissionsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.FlyingMissions.View, PermissionCodes.FlyingMissions.Query, PermissionCodes.FlyingMissions.Create, PermissionCodes.FlyingMissions.Update, PermissionCodes.FlyingMissions.Delete)]
 public async Task<IActionResult> List([FromQuery] GetFlyingMissionDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.FlyingMissions.View, PermissionCodes.FlyingMissions.Query, PermissionCodes.FlyingMissions.Create, PermissionCodes.FlyingMissions.Update, PermissionCodes.FlyingMissions.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetFlyingMissionDefByIdQuery(id),ct));
 
 [HttpGet("export")]
 [HasPermission(PermissionCodes.FlyingMissions.Query)]
 public async Task<IActionResult> Export([FromQuery] GetFlyingMissionDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreleyerek en fazla 5000 kayıt indirin."});
   var rows=new List<FlyingMissionDefResponse>(first.Items);
   for(var page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
      rows.AddRange((await sender.Send(q with{PageNumber=page,PageSize=100},ct)).Items);
   ExcelColumn<FlyingMissionDefResponse>[] columns={new("Kod",x=>x.Code,Width:17),new("Ad",x=>x.Name,Width:42),new("Geçerli Başlangıç",x=>x.ValidFrom,NumberFormat:"dd.MM.yyyy HH:mm"),new("Geçerli Bitiş",x=>x.ValidTo,NumberFormat:"dd.MM.yyyy HH:mm"),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(rows,columns,"Uçucu Görev Tanımları"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "flying-missions_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpPost]
 [HasPermission(PermissionCodes.FlyingMissions.Create)]
 public async Task<IActionResult> Create([FromBody] CreateFlyingMissionDefCommand r,CancellationToken ct)
 {
   var item=await sender.Send(r,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.FlyingMissions.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateFlyingMissionDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.FlyingMissions.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusFlyingMissionDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.FlyingMissions.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteFlyingMissionDefCommand(id,version),ct);
   return NoContent();
 }
}
