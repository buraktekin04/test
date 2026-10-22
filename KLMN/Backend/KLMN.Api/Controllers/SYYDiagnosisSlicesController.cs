using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYDiagnosisSlices.Responses;
using KLMN.Application.SYYDiagnosisSlices.Commands.CreateSYYDiagnosisSliceDef;
using KLMN.Application.SYYDiagnosisSlices.Commands.UpdateSYYDiagnosisSliceDef;
using KLMN.Application.SYYDiagnosisSlices.Commands.ChangeStatusSYYDiagnosisSliceDef;
using KLMN.Application.SYYDiagnosisSlices.Commands.DeleteSYYDiagnosisSliceDef;
using KLMN.Application.SYYDiagnosisSlices.Queries.GetSYYDiagnosisSliceDefs;
using KLMN.Application.SYYDiagnosisSlices.Queries.GetSYYDiagnosisSliceDefById;

using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>SYY Dilim Tanımları CRUD, Excel ve izin kontrollü API.</summary>
[ApiController]
[Route("api/syy-diagnosis-slices")]
public sealed class SYYDiagnosisSlicesController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYDiagnosisSlices.View, PermissionCodes.SYYDiagnosisSlices.Query, PermissionCodes.SYYDiagnosisSlices.Create, PermissionCodes.SYYDiagnosisSlices.Update, PermissionCodes.SYYDiagnosisSlices.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYDiagnosisSliceDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisSlices.View, PermissionCodes.SYYDiagnosisSlices.Query, PermissionCodes.SYYDiagnosisSlices.Create, PermissionCodes.SYYDiagnosisSlices.Update, PermissionCodes.SYYDiagnosisSlices.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetSYYDiagnosisSliceDefByIdQuery(id),ct));
 
 [HttpGet("export")]
 [HasPermission(PermissionCodes.SYYDiagnosisSlices.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYDiagnosisSliceDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreleyerek en fazla 5000 kayıt indirin."});
   var rows=new List<SYYDiagnosisSliceDefResponse>(first.Items);
   for(var page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
      rows.AddRange((await sender.Send(q with{PageNumber=page,PageSize=100},ct)).Items);
   ExcelColumn<SYYDiagnosisSliceDefResponse>[] columns={new("Kod",x=>x.Code,Width:17),new("Ad",x=>x.Name,Width:42),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(rows,columns,"SYY Dilim Tanımları"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "syy-diagnosis-slices_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpPost]
 [HasPermission(PermissionCodes.SYYDiagnosisSlices.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYDiagnosisSliceDefCommand r,CancellationToken ct)
 {
   var item=await sender.Send(r,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisSlices.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYDiagnosisSliceDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.SYYDiagnosisSlices.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYDiagnosisSliceDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisSlices.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteSYYDiagnosisSliceDefCommand(id,version),ct);
   return NoContent();
 }
}
