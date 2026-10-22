using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYDiagnosisBranches.Responses;
using KLMN.Application.SYYDiagnosisBranches.Commands.CreateSYYDiagnosisBranchDef;
using KLMN.Application.SYYDiagnosisBranches.Commands.UpdateSYYDiagnosisBranchDef;
using KLMN.Application.SYYDiagnosisBranches.Commands.ChangeStatusSYYDiagnosisBranchDef;
using KLMN.Application.SYYDiagnosisBranches.Commands.DeleteSYYDiagnosisBranchDef;
using KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefs;
using KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefById;
using KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>SYY Gereken Branşlar: ayrı CRUD, seçenekler, audit ve Excel endpointleri.</summary>
[ApiController,Route("api/syy-diagnosis-branches")]
public sealed class SYYDiagnosisBranchesController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.View,PermissionCodes.SYYDiagnosisBranches.Query,PermissionCodes.SYYDiagnosisBranches.Create,PermissionCodes.SYYDiagnosisBranches.Update,PermissionCodes.SYYDiagnosisBranches.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYDiagnosisBranchDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));

 [HttpGet("options")]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.View,PermissionCodes.SYYDiagnosisBranches.Query,PermissionCodes.SYYDiagnosisBranches.Create,PermissionCodes.SYYDiagnosisBranches.Update,PermissionCodes.SYYDiagnosisBranches.Delete)]
 public async Task<IActionResult> Options([FromQuery]Guid? selectedSYYDiagnosisDefId,[FromQuery]Guid? selectedBranchDefId,CancellationToken ct)=>
 Ok(await sender.Send(new GetSYYDiagnosisBranchDefOptionsQuery(selectedSYYDiagnosisDefId,selectedBranchDefId),ct));

 [HttpGet("export")]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYDiagnosisBranchDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="En fazla 5000 kayıt dışa aktarılabilir."});
   var items=new List<SYYDiagnosisBranchDefResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
     items.AddRange((await sender.Send(q with{PageNumber=i,PageSize=100},ct)).Items);
   ExcelColumn<SYYDiagnosisBranchDefResponse>[] cols=[new("SYY Tanısı",x=>x.DiagnosisName,Width:26),
new("Branş",x=>x.BranchName,Width:26),
new("Başlangıç Yaşı",x=>x.StartingAge,Width:26),
new("Bitiş Yaşı",x=>x.EndingAge,Width:26),
new("Cinsiyet Kodu",x=>x.Gender,Width:26),
     new("Durum",x=>x.IsActive?"Aktif":"Pasif")];
   return File(excel.Export(items,cols,"SYY Gereken Branşlar"),
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "syy-diagnosis-branches_"+DateTime.UtcNow.ToString("yyyyMMdd")+".xlsx");
 }

 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.View,PermissionCodes.SYYDiagnosisBranches.Query,PermissionCodes.SYYDiagnosisBranches.Create,PermissionCodes.SYYDiagnosisBranches.Update,PermissionCodes.SYYDiagnosisBranches.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetSYYDiagnosisBranchDefByIdQuery(id),ct));

 [HttpPost]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYDiagnosisBranchDefCommand r,CancellationToken ct)
 {
  var result=await sender.Send(r,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYDiagnosisBranchDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYDiagnosisBranchDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnosisBranches.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct)
 {
  await sender.Send(new DeleteSYYDiagnosisBranchDefCommand(id,version),ct);return NoContent();
 }
}
