using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.AnnotationReasons.Responses;
using KLMN.Application.AnnotationReasons.Commands.CreateAnnotationReasonDef;
using KLMN.Application.AnnotationReasons.Commands.UpdateAnnotationReasonDef;
using KLMN.Application.AnnotationReasons.Commands.ChangeStatusAnnotationReasonDef;
using KLMN.Application.AnnotationReasons.Commands.DeleteAnnotationReasonDef;
using KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefs;
using KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefById;

using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Zeyil Sebebi Tanımları CRUD, Excel ve izin kontrollü API.</summary>
[ApiController]
[Route("api/annotation-reasons")]
public sealed class AnnotationReasonsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.AnnotationReasons.View, PermissionCodes.AnnotationReasons.Query, PermissionCodes.AnnotationReasons.Create, PermissionCodes.AnnotationReasons.Update, PermissionCodes.AnnotationReasons.Delete)]
 public async Task<IActionResult> List([FromQuery] GetAnnotationReasonDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.AnnotationReasons.View, PermissionCodes.AnnotationReasons.Query, PermissionCodes.AnnotationReasons.Create, PermissionCodes.AnnotationReasons.Update, PermissionCodes.AnnotationReasons.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetAnnotationReasonDefByIdQuery(id),ct));
 
 [HttpGet("export")]
 [HasPermission(PermissionCodes.AnnotationReasons.Query)]
 public async Task<IActionResult> Export([FromQuery] GetAnnotationReasonDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreleyerek en fazla 5000 kayıt indirin."});
   var rows=new List<AnnotationReasonDefResponse>(first.Items);
   for(var page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
      rows.AddRange((await sender.Send(q with{PageNumber=page,PageSize=100},ct)).Items);
   ExcelColumn<AnnotationReasonDefResponse>[] columns={new("Kod",x=>x.Code,Width:17),new("Ad",x=>x.Name,Width:42),new("Geçerli Başlangıç",x=>x.ValidFrom,NumberFormat:"dd.MM.yyyy HH:mm"),new("Geçerli Bitiş",x=>x.ValidTo,NumberFormat:"dd.MM.yyyy HH:mm"),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(rows,columns,"Zeyil Sebebi Tanımları"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "annotation-reasons_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpPost]
 [HasPermission(PermissionCodes.AnnotationReasons.Create)]
 public async Task<IActionResult> Create([FromBody] CreateAnnotationReasonDefCommand r,CancellationToken ct)
 {
   var item=await sender.Send(r,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.AnnotationReasons.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateAnnotationReasonDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.AnnotationReasons.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusAnnotationReasonDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.AnnotationReasons.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteAnnotationReasonDefCommand(id,version),ct);
   return NoContent();
 }
}
