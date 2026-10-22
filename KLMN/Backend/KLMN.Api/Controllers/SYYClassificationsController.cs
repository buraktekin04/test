using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYClassifications.Responses;
using KLMN.Application.SYYClassifications.Commands.CreateSYYClassificationDef;
using KLMN.Application.SYYClassifications.Commands.UpdateSYYClassificationDef;
using KLMN.Application.SYYClassifications.Commands.ChangeStatusSYYClassificationDef;
using KLMN.Application.SYYClassifications.Commands.DeleteSYYClassificationDef;
using KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefs;
using KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefById;
using KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: ayrı CRUD, seçenekler, audit ve Excel endpointleri.</summary>
[ApiController,Route("api/syy-classifications")]
public sealed class SYYClassificationsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYClassifications.View,PermissionCodes.SYYClassifications.Query,PermissionCodes.SYYClassifications.Create,PermissionCodes.SYYClassifications.Update,PermissionCodes.SYYClassifications.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYClassificationDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));

 [HttpGet("options")]
 [HasPermission(PermissionCodes.SYYClassifications.View,PermissionCodes.SYYClassifications.Query,PermissionCodes.SYYClassifications.Create,PermissionCodes.SYYClassifications.Update,PermissionCodes.SYYClassifications.Delete)]
 public async Task<IActionResult> Options([FromQuery]Guid? selectedSYYDiagnosisDefId,[FromQuery]Guid? selectedForcesCommandDef,[FromQuery]Guid? selectedRankGroup,[FromQuery]Guid? selectedMilitaryClassGroup,[FromQuery]Guid? selectedHcDecision,CancellationToken ct)=>
 Ok(await sender.Send(new GetSYYClassificationDefOptionsQuery(selectedSYYDiagnosisDefId,selectedForcesCommandDef,selectedRankGroup,selectedMilitaryClassGroup,selectedHcDecision),ct));

 [HttpGet("export")]
 [HasPermission(PermissionCodes.SYYClassifications.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYClassificationDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="En fazla 5000 kayıt dışa aktarılabilir."});
   var items=new List<SYYClassificationDefResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
     items.AddRange((await sender.Send(q with{PageNumber=i,PageSize=100},ct)).Items);
   ExcelColumn<SYYClassificationDefResponse>[] cols=[new("SYY Tanısı",x=>x.DiagnosisName,Width:26),
new("Kuvvet",x=>x.ForcesCommandName,Width:26),
new("Rütbe Grubu",x=>x.RankGroupDefinitionName,Width:26),
new("Sınıf Grubu",x=>x.MilitaryClassGroupDefinitionName,Width:26),
new("Karar",x=>x.DecisionName,Width:26),
new("Geçerli Başlangıç",x=>x.ValidFrom,Width:26),
new("Geçerli Bitiş",x=>x.ValidTo,Width:26),
     new("Durum",x=>x.IsActive?"Aktif":"Pasif")];
   return File(excel.Export(items,cols,"SYY Tanı Sınıflandırma Çizelgesi"),
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "syy-classifications_"+DateTime.UtcNow.ToString("yyyyMMdd")+".xlsx");
 }

 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.SYYClassifications.View,PermissionCodes.SYYClassifications.Query,PermissionCodes.SYYClassifications.Create,PermissionCodes.SYYClassifications.Update,PermissionCodes.SYYClassifications.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetSYYClassificationDefByIdQuery(id),ct));

 [HttpPost]
 [HasPermission(PermissionCodes.SYYClassifications.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYClassificationDefCommand r,CancellationToken ct)
 {
  var result=await sender.Send(r,ct);return CreatedAtAction(nameof(ById),new{id=result.Id},result);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.SYYClassifications.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYClassificationDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.SYYClassifications.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYClassificationDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik uyuşmazlığı.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.SYYClassifications.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct)
 {
  await sender.Send(new DeleteSYYClassificationDefCommand(id,version),ct);return NoContent();
 }
}
