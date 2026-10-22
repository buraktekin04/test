using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
using KLMN.Application.SYYDiagnosisProcedures.Commands.CreateSYYDiagnosisProcedureDef;
using KLMN.Application.SYYDiagnosisProcedures.Commands.UpdateSYYDiagnosisProcedureDef;
using KLMN.Application.SYYDiagnosisProcedures.Commands.ChangeStatusSYYDiagnosisProcedureDef;
using KLMN.Application.SYYDiagnosisProcedures.Commands.DeleteSYYDiagnosisProcedureDef;
using KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefs;
using KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefById;
using KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>SYY Gereken Tetkikler: ayrı CRUD, seçenekler, audit ve Excel endpointleri.</summary>
[ApiController,Route("api/syy-diagnosis-procedures")]
public sealed class SYYDiagnosisProceduresController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.View,PermissionCodes.SYYDiagnosisProcedures.Query,PermissionCodes.SYYDiagnosisProcedures.Create,PermissionCodes.SYYDiagnosisProcedures.Update,PermissionCodes.SYYDiagnosisProcedures.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYDiagnosisProcedureDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));

 [HttpGet("options")]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.View,PermissionCodes.SYYDiagnosisProcedures.Query,PermissionCodes.SYYDiagnosisProcedures.Create,PermissionCodes.SYYDiagnosisProcedures.Update,PermissionCodes.SYYDiagnosisProcedures.Delete)]
 public async Task<IActionResult> Options([FromQuery]Guid? selectedSYYDiagnosisDefId,[FromQuery]Guid? selectedProcedureDefId,CancellationToken ct)=>
 Ok(await sender.Send(new GetSYYDiagnosisProcedureDefOptionsQuery(selectedSYYDiagnosisDefId,selectedProcedureDefId),ct));

 [HttpGet("export")]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYDiagnosisProcedureDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="En fazla 5000 kayıt dışa aktarılabilir."});
   var items=new List<SYYDiagnosisProcedureDefResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
     items.AddRange((await sender.Send(q with{PageNumber=i,PageSize=100},ct)).Items);
   ExcelColumn<SYYDiagnosisProcedureDefResponse>[] cols=[new("SYY Tanısı",x=>x.DiagnosisName,Width:26),
new("Tetkik",x=>x.ProcedureName,Width:26),
new("Başlangıç Yaşı",x=>x.StartingAge,Width:26),
new("Bitiş Yaşı",x=>x.EndingAge,Width:26),
new("Cinsiyet Kodu",x=>x.Gender,Width:26),
     new("Durum",x=>x.IsActive?"Aktif":"Pasif")];
   return File(excel.Export(items,cols,"SYY Gereken Tetkikler"),
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "syy-diagnosis-procedures_"+DateTime.UtcNow.ToString("yyyyMMdd")+".xlsx");
 }

 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.View,PermissionCodes.SYYDiagnosisProcedures.Query,PermissionCodes.SYYDiagnosisProcedures.Create,PermissionCodes.SYYDiagnosisProcedures.Update,PermissionCodes.SYYDiagnosisProcedures.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetSYYDiagnosisProcedureDefByIdQuery(id),ct));

 [HttpPost]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYDiagnosisProcedureDefCommand r,CancellationToken ct)
 {
  var result=await sender.Send(r,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYDiagnosisProcedureDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYDiagnosisProcedureDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisProcedures.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct)
 {
  await sender.Send(new DeleteSYYDiagnosisProcedureDefCommand(id,version),ct);return NoContent();
 }
}
