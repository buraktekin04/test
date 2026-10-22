using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYDiagnosisTypes.Responses;
using KLMN.Application.SYYDiagnosisTypes.Commands.CreateSYYDiagnosisTypeDef;
using KLMN.Application.SYYDiagnosisTypes.Commands.UpdateSYYDiagnosisTypeDef;
using KLMN.Application.SYYDiagnosisTypes.Commands.ChangeStatusSYYDiagnosisTypeDef;
using KLMN.Application.SYYDiagnosisTypes.Commands.DeleteSYYDiagnosisTypeDef;
using KLMN.Application.SYYDiagnosisTypes.Queries.GetSYYDiagnosisTypeDefs;
using KLMN.Application.SYYDiagnosisTypes.Queries.GetSYYDiagnosisTypeDefById;

using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>SYY Tanı Türü Tanımları CRUD, Excel ve izin kontrollü API.</summary>
[ApiController]
[Route("api/syy-diagnosis-types")]
public sealed class SYYDiagnosisTypesController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYDiagnosisTypes.View, PermissionCodes.SYYDiagnosisTypes.Query, PermissionCodes.SYYDiagnosisTypes.Create, PermissionCodes.SYYDiagnosisTypes.Update, PermissionCodes.SYYDiagnosisTypes.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYDiagnosisTypeDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisTypes.View, PermissionCodes.SYYDiagnosisTypes.Query, PermissionCodes.SYYDiagnosisTypes.Create, PermissionCodes.SYYDiagnosisTypes.Update, PermissionCodes.SYYDiagnosisTypes.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetSYYDiagnosisTypeDefByIdQuery(id),ct));
 
 [HttpGet("export")]
 [HasPermission(PermissionCodes.SYYDiagnosisTypes.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYDiagnosisTypeDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreleyerek en fazla 5000 kayıt indirin."});
   var rows=new List<SYYDiagnosisTypeDefResponse>(first.Items);
   for(var page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
      rows.AddRange((await sender.Send(q with{PageNumber=page,PageSize=100},ct)).Items);
   ExcelColumn<SYYDiagnosisTypeDefResponse>[] columns={new("Kod",x=>x.Code,Width:17),new("Ad",x=>x.Name,Width:42),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(rows,columns,"SYY Tanı Türü Tanımları"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "syy-diagnosis-types_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpPost]
 [HasPermission(PermissionCodes.SYYDiagnosisTypes.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYDiagnosisTypeDefCommand r,CancellationToken ct)
 {
   var item=await sender.Send(r,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisTypes.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYDiagnosisTypeDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.SYYDiagnosisTypes.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYDiagnosisTypeDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisTypes.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteSYYDiagnosisTypeDefCommand(id,version),ct);
   return NoContent();
 }
}
