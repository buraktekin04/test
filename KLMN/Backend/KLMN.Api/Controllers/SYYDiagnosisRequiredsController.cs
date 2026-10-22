using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
using KLMN.Application.SYYDiagnosisRequireds.Commands.CreateSYYDiagnosisRequired;
using KLMN.Application.SYYDiagnosisRequireds.Commands.UpdateSYYDiagnosisRequired;
using KLMN.Application.SYYDiagnosisRequireds.Commands.ChangeStatusSYYDiagnosisRequired;
using KLMN.Application.SYYDiagnosisRequireds.Commands.DeleteSYYDiagnosisRequired;
using KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequireds;
using KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequiredById;
using KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequiredOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>SYY Onay Gerektiren Maddeler: ayrı CRUD, seçenekler, audit ve Excel endpointleri.</summary>
[ApiController,Route("api/syy-diagnosis-requireds")]
public sealed class SYYDiagnosisRequiredsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.View,PermissionCodes.SYYDiagnosisRequireds.Query,PermissionCodes.SYYDiagnosisRequireds.Create,PermissionCodes.SYYDiagnosisRequireds.Update,PermissionCodes.SYYDiagnosisRequireds.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYDiagnosisRequiredsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));

 [HttpGet("options")]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.View,PermissionCodes.SYYDiagnosisRequireds.Query,PermissionCodes.SYYDiagnosisRequireds.Create,PermissionCodes.SYYDiagnosisRequireds.Update,PermissionCodes.SYYDiagnosisRequireds.Delete)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedDiagnosisId,CancellationToken ct)=>
 Ok(await sender.Send(new GetSYYDiagnosisRequiredOptionsQuery(selectedDiagnosisId),ct));

 [HttpGet("export")]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYDiagnosisRequiredsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="En fazla 5000 kayıt dışa aktarılabilir."});
   var items=new List<SYYDiagnosisRequiredResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
     items.AddRange((await sender.Send(q with{PageNumber=i,PageSize=100},ct)).Items);
   ExcelColumn<SYYDiagnosisRequiredResponse>[] cols=[new("SYY Tanısı",x=>x.DiagnosisName,Width:26),
new("Tümü Zorunlu",x=>x.MustAll,Width:26),
new("En Az Biri",x=>x.MustOneOf,Width:26),
new("Madde Açıklamaları",x=>string.Join(" | ",x.Details.OrderBy(d=>d.SortOrder).Select(d=>d.Description)),Width:60),
     new("Durum",x=>x.IsActive?"Aktif":"Pasif")];
   return File(excel.Export(items,cols,"SYY Onay Gerektiren Maddeler"),
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "syy-diagnosis-requireds_"+DateTime.UtcNow.ToString("yyyyMMdd")+".xlsx");
 }

 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.View,PermissionCodes.SYYDiagnosisRequireds.Query,PermissionCodes.SYYDiagnosisRequireds.Create,PermissionCodes.SYYDiagnosisRequireds.Update,PermissionCodes.SYYDiagnosisRequireds.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetSYYDiagnosisRequiredByIdQuery(id),ct));

 [HttpPost]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYDiagnosisRequiredCommand r,CancellationToken ct)
 {
  var result=await sender.Send(r,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYDiagnosisRequiredCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYDiagnosisRequiredCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisRequireds.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct)
 {
  await sender.Send(new DeleteSYYDiagnosisRequiredCommand(id,version),ct);return NoContent();
 }
}
