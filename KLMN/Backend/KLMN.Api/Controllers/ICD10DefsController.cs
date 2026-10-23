using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ICD10Defs.Responses;
using KLMN.Application.ICD10Defs.Commands.CreateICD10Def;
using KLMN.Application.ICD10Defs.Commands.UpdateICD10Def;
using KLMN.Application.ICD10Defs.Commands.ChangeStatusICD10Def;
using KLMN.Application.ICD10Defs.Commands.DeleteICD10Def;
using KLMN.Application.ICD10Defs.Queries.GetICD10Defs;
using KLMN.Application.ICD10Defs.Queries.GetICD10DefById;

using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>ICD10 Tanımları: ayrı CQRS, audit, Excel, permission ve xmin API.</summary>
[ApiController,Route("api/icd10-defs")]
public sealed class ICD10DefsController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.ICD10Defs.View, PermissionCodes.ICD10Defs.Query, PermissionCodes.ICD10Defs.Create, PermissionCodes.ICD10Defs.Update, PermissionCodes.ICD10Defs.Delete)]
 public async Task<IActionResult> List([FromQuery] GetICD10DefsQuery query,CancellationToken ct)=>
  Ok(await sender.Send(query,ct));
 
 [HttpGet("export")]
 [HasPermission(PermissionCodes.ICD10Defs.Query)]
 public async Task<IActionResult> Export([FromQuery]GetICD10DefsQuery q,CancellationToken ct)
 {
  var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
  if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,
    Title="Excel kayıt limiti aşıldı.",Detail="En fazla 5000 kayıt dışa aktarılabilir."});
  var list=new List<ICD10DefResponse>(first.Items);
  for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
    list.AddRange((await sender.Send(q with{PageNumber=i,PageSize=100},ct)).Items);
  ExcelColumn<ICD10DefResponse>[] columns=[
    
 new("ICD10 Kodu",x=>x.Code,Width:20),
 new("Adı",x=>x.Name,Width:55),
    new("Durum",x=>x.IsActive?"Aktif":"Pasif")
  ];
  return File(excel.Export(list,columns,"ICD10 Tanımları"),
   "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
   "icd10-defs_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.ICD10Defs.View, PermissionCodes.ICD10Defs.Query, PermissionCodes.ICD10Defs.Create, PermissionCodes.ICD10Defs.Update, PermissionCodes.ICD10Defs.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>
   Ok(await sender.Send(new GetICD10DefByIdQuery(id),ct));
 [HttpPost]
 [HasPermission(PermissionCodes.ICD10Defs.Create)]
 public async Task<IActionResult> Create([FromBody]CreateICD10DefCommand r,CancellationToken ct)
 {
  var record=await sender.Send(r,ct);
  return CreatedAtAction(nameof(ById),new{id=record.Id},record);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.ICD10Defs.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody]UpdateICD10DefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.ICD10Defs.Update)]
 public async Task<IActionResult> ChangeStatus(Guid id,[FromBody]ChangeStatusICD10DefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.ICD10Defs.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery]uint version,CancellationToken ct)
 {
  await sender.Send(new DeleteICD10DefCommand(id,version),ct);
  return NoContent();
 }
}
