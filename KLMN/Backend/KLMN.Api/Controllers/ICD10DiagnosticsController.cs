using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ICD10Diagnostics.Responses;
using KLMN.Application.ICD10Diagnostics.Commands.CreateICD10Diagnostic;
using KLMN.Application.ICD10Diagnostics.Commands.UpdateICD10Diagnostic;
using KLMN.Application.ICD10Diagnostics.Commands.ChangeStatusICD10Diagnostic;
using KLMN.Application.ICD10Diagnostics.Commands.DeleteICD10Diagnostic;
using KLMN.Application.ICD10Diagnostics.Queries.GetICD10Diagnostics;
using KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticById;
using KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: ayrı CQRS, audit, Excel, permission ve xmin API.</summary>
[ApiController,Route("api/icd10-diagnostics")]
public sealed class ICD10DiagnosticsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.ICD10Diagnostics.View, PermissionCodes.ICD10Diagnostics.Query, PermissionCodes.ICD10Diagnostics.Create, PermissionCodes.ICD10Diagnostics.Update, PermissionCodes.ICD10Diagnostics.Delete)]
 public async Task<IActionResult> List([FromQuery] GetICD10DiagnosticsQuery query,CancellationToken ct)=>
  Ok(await sender.Send(query,ct));
 
 [HttpGet("options")]
 [HasPermission(PermissionCodes.ICD10Diagnostics.View, PermissionCodes.ICD10Diagnostics.Query, PermissionCodes.ICD10Diagnostics.Create, PermissionCodes.ICD10Diagnostics.Update, PermissionCodes.ICD10Diagnostics.Delete)]
 public async Task<IActionResult> Options([FromQuery]Guid? selectedICD10DefId,
 [FromQuery]Guid? selectedSYYDiagnosisDefId,[FromQuery]Guid? selectedHcAdmissionTypeDefId,CancellationToken ct)=>
  Ok(await sender.Send(new GetICD10DiagnosticOptionsQuery(
    selectedICD10DefId,selectedSYYDiagnosisDefId,selectedHcAdmissionTypeDefId),ct));
 [HttpGet("export")]
 [HasPermission(PermissionCodes.ICD10Diagnostics.Query)]
 public async Task<IActionResult> Export([FromQuery]GetICD10DiagnosticsQuery q,CancellationToken ct)
 {
  var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
  if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,
    Title="Excel kayıt limiti aşıldı.",Detail="En fazla 5000 kayıt dışa aktarılabilir."});
  var list=new List<ICD10DiagnosticResponse>(first.Items);
  for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
    list.AddRange((await sender.Send(q with{PageNumber=i,PageSize=100},ct)).Items);
  ExcelColumn<ICD10DiagnosticResponse>[] columns=[
    
 new("ICD10 Kodu",x=>x.ICD10Code,Width:18),
 new("ICD10 Adı",x=>x.ICD10Name,Width:40),
 new("SYY Tanısı",x=>x.SYYDiagnosisName,Width:40),
 new("Başvuru Grubu",x=>x.HcAdmissionTypeName,Width:36),
 new("Adet",x=>x.Amount,Width:15),
    new("Durum",x=>x.IsActive?"Aktif":"Pasif")
  ];
  return File(excel.Export(list,columns,"ICD10 SYY Tanı İstatistik Tanımları"),
   "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
   "icd10-diagnostics_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.ICD10Diagnostics.View, PermissionCodes.ICD10Diagnostics.Query, PermissionCodes.ICD10Diagnostics.Create, PermissionCodes.ICD10Diagnostics.Update, PermissionCodes.ICD10Diagnostics.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>
   Ok(await sender.Send(new GetICD10DiagnosticByIdQuery(id),ct));
 [HttpPost]
 [HasPermission(PermissionCodes.ICD10Diagnostics.Create)]
 public async Task<IActionResult> Create([FromBody]CreateICD10DiagnosticCommand r,CancellationToken ct)
 {
  var record=await sender.Send(r,ct);
  return CreatedAtAction(nameof(ById),new{id=record.Id},record);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.ICD10Diagnostics.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody]UpdateICD10DiagnosticCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.ICD10Diagnostics.Update)]
 public async Task<IActionResult> ChangeStatus(Guid id,[FromBody]ChangeStatusICD10DiagnosticCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.ICD10Diagnostics.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct)
 {
  await sender.Send(new DeleteICD10DiagnosticCommand(id,version),ct);
  return NoContent();
 }
}
