using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
using KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisions;
using KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisionOptions;
using KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisionById;
using KLMN.Application.SYYDiagnosisDecisions.Commands.CreateSYYDiagnosisDecision;
using KLMN.Application.SYYDiagnosisDecisions.Commands.UpdateSYYDiagnosisDecision;
using KLMN.Application.SYYDiagnosisDecisions.Commands.ChangeStatusSYYDiagnosisDecision;
using KLMN.Application.SYYDiagnosisDecisions.Commands.DeleteSYYDiagnosisDecision;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Karar istatistiği CRUD ile salt-okunur üç dropdown'lı sorgulama ve Excel.</summary>
[ApiController]
[Route("api/syy-diagnosis-decisions")]
public sealed class SYYDiagnosisDecisionsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet][HasPermission(PermissionCodes.SYYDiagnosisDecisions.View, PermissionCodes.SYYDiagnosisDecisions.Query, PermissionCodes.SYYDiagnosisDecisions.Create, PermissionCodes.SYYDiagnosisDecisions.Update, PermissionCodes.SYYDiagnosisDecisions.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYDiagnosisDecisionsQuery q,CancellationToken ct)
  =>Ok(await sender.Send(q,ct));
 [HttpGet("search")][HasPermission(PermissionCodes.SYYDiagnosisDecisions.Query)]
 public async Task<IActionResult> Search([FromQuery] GetSYYDiagnosisDecisionsQuery q,CancellationToken ct)
  =>Ok(await sender.Send(q,ct));
 [HttpGet("options")][HasPermission(PermissionCodes.SYYDiagnosisDecisions.View, PermissionCodes.SYYDiagnosisDecisions.Query, PermissionCodes.SYYDiagnosisDecisions.Create, PermissionCodes.SYYDiagnosisDecisions.Update, PermissionCodes.SYYDiagnosisDecisions.Delete)]
 public async Task<IActionResult> Options(
  [FromQuery] Guid? selectedSYYDiagnosisDefId,[FromQuery] Guid? selectedHcDecisionDefId,
  [FromQuery] Guid? selectedHcAdmissionTypeDefId,[FromQuery] Guid? selectedMilitaryClassDefId,[FromQuery] bool includeInactive,CancellationToken ct)
 =>Ok(await sender.Send(new GetSYYDiagnosisDecisionOptionsQuery(selectedSYYDiagnosisDefId,selectedHcDecisionDefId,selectedHcAdmissionTypeDefId,selectedMilitaryClassDefId,includeInactive),ct));
 [HttpGet("export")][HasPermission(PermissionCodes.SYYDiagnosisDecisions.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYDiagnosisDecisionsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel kayıt sınırı aşıldı.",Detail="En fazla 5000 kayıt aktarılabilir."});
   var all=new List<SYYDiagnosisDecisionResponse>(first.Items);
   for(var p=2;p<=(int)Math.Ceiling(first.TotalCount/100d);p++)
      all.AddRange((await sender.Send(q with{PageNumber=p,PageSize=100},ct)).Items);
   var columns=new ExcelColumn<SYYDiagnosisDecisionResponse>[] {
 new("SYY Tanı Kodu",x=>x.SYYDiagnosisCode,Width:18),new("SYY Tanısı",x=>x.SYYDiagnosisName,Width:45),
 new("Kurul Kararı Kodu",x=>x.HcDecisionCode,Width:18),new("Kurul Kararı",x=>x.HcDecisionName,Width:44),
 new("Başvuru Grubu",x=>x.HcAdmissionTypeName,Width:34),new("Askerî Sınıf",x=>x.MilitaryClassName??"Belirtilmedi",Width:30),
 new("Adet",x=>x.Amount,Width:12),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(all,columns,"SYY Tanı Karar İstatistiği"),
       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
       "syy_tani_karar_istatistigi_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")][HasPermission(PermissionCodes.SYYDiagnosisDecisions.View, PermissionCodes.SYYDiagnosisDecisions.Query, PermissionCodes.SYYDiagnosisDecisions.Create, PermissionCodes.SYYDiagnosisDecisions.Update, PermissionCodes.SYYDiagnosisDecisions.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)
  =>Ok(await sender.Send(new GetSYYDiagnosisDecisionByIdQuery(id),ct));
 [HttpPost][HasPermission(PermissionCodes.SYYDiagnosisDecisions.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYDiagnosisDecisionCommand cmd,CancellationToken ct)
 {
   var result=await sender.Send(cmd,ct);
   return CreatedAtAction(nameof(ById),new{id=result.Id},result);
 }
 [HttpPut("{id:guid}")][HasPermission(PermissionCodes.SYYDiagnosisDecisions.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYDiagnosisDecisionCommand cmd,CancellationToken ct)
 {
   if(id!=cmd.Id)return BadRequest("Kayıt kimliği uyuşmuyor.");
   return Ok(await sender.Send(cmd,ct));
 }
 [HttpPatch("{id:guid}/status")][HasPermission(PermissionCodes.SYYDiagnosisDecisions.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYDiagnosisDecisionCommand cmd,CancellationToken ct)
 {
   if(id!=cmd.Id)return BadRequest("Kayıt kimliği uyuşmuyor.");
   return Ok(await sender.Send(cmd,ct));
 }
 [HttpDelete("{id:guid}")][HasPermission(PermissionCodes.SYYDiagnosisDecisions.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteSYYDiagnosisDecisionCommand(id,version),ct);
   return NoContent();
 }
}
