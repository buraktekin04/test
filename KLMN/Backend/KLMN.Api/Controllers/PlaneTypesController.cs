using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.PlaneTypes.Responses;
using KLMN.Application.PlaneTypes.Commands.CreatePlaneTypeDef;
using KLMN.Application.PlaneTypes.Commands.UpdatePlaneTypeDef;
using KLMN.Application.PlaneTypes.Commands.ChangeStatusPlaneTypeDef;
using KLMN.Application.PlaneTypes.Commands.DeletePlaneTypeDef;
using KLMN.Application.PlaneTypes.Queries.GetPlaneTypeDefs;
using KLMN.Application.PlaneTypes.Queries.GetPlaneTypeDefById;

using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Uçtuğu Uçak Tipi Tanımları CRUD, Excel ve izin kontrollü API.</summary>
[ApiController]
[Route("api/plane-types")]
public sealed class PlaneTypesController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.PlaneTypes.View, PermissionCodes.PlaneTypes.Query, PermissionCodes.PlaneTypes.Create, PermissionCodes.PlaneTypes.Update, PermissionCodes.PlaneTypes.Delete)]
 public async Task<IActionResult> List([FromQuery] GetPlaneTypeDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.PlaneTypes.View, PermissionCodes.PlaneTypes.Query, PermissionCodes.PlaneTypes.Create, PermissionCodes.PlaneTypes.Update, PermissionCodes.PlaneTypes.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetPlaneTypeDefByIdQuery(id),ct));
 
 [HttpGet("export")]
 [HasPermission(PermissionCodes.PlaneTypes.Query)]
 public async Task<IActionResult> Export([FromQuery] GetPlaneTypeDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreleyerek en fazla 5000 kayıt indirin."});
   var rows=new List<PlaneTypeDefResponse>(first.Items);
   for(var page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
      rows.AddRange((await sender.Send(q with{PageNumber=page,PageSize=100},ct)).Items);
   ExcelColumn<PlaneTypeDefResponse>[] columns={new("Kod",x=>x.Code,Width:17),new("Ad",x=>x.Name,Width:42),new("Geçerli Başlangıç",x=>x.ValidFrom,NumberFormat:"dd.MM.yyyy HH:mm"),new("Geçerli Bitiş",x=>x.ValidTo,NumberFormat:"dd.MM.yyyy HH:mm"),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(rows,columns,"Uçtuğu Uçak Tipi Tanımları"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "plane-types_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpPost]
 [HasPermission(PermissionCodes.PlaneTypes.Create)]
 public async Task<IActionResult> Create([FromBody] CreatePlaneTypeDefCommand r,CancellationToken ct)
 {
   var item=await sender.Send(r,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.PlaneTypes.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdatePlaneTypeDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.PlaneTypes.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusPlaneTypeDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.PlaneTypes.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeletePlaneTypeDefCommand(id,version),ct);
   return NoContent();
 }
}
