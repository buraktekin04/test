using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.SYYDiagnoses.Responses;
using KLMN.Application.SYYDiagnoses.Commands.CreateSYYDiagnosisDef;
using KLMN.Application.SYYDiagnoses.Commands.UpdateSYYDiagnosisDef;
using KLMN.Application.SYYDiagnoses.Commands.ChangeStatusSYYDiagnosisDef;
using KLMN.Application.SYYDiagnoses.Commands.DeleteSYYDiagnosisDef;
using KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefs;
using KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefById;
using KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>SYY Tanı Listesi (M/D/F) CRUD, Excel ve izin kontrollü API.</summary>
[ApiController]
[Route("api/syy-diagnoses")]
public sealed class SYYDiagnosesController(ISender sender,IExcelExportService excel):ControllerBase
{
 [HttpGet]
 [HasPermission(PermissionCodes.SYYDiagnoses.View, PermissionCodes.SYYDiagnoses.Query, PermissionCodes.SYYDiagnoses.Create, PermissionCodes.SYYDiagnoses.Update, PermissionCodes.SYYDiagnoses.Delete)]
 public async Task<IActionResult> List([FromQuery] GetSYYDiagnosisDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnoses.View, PermissionCodes.SYYDiagnoses.Query, PermissionCodes.SYYDiagnoses.Create, PermissionCodes.SYYDiagnoses.Update, PermissionCodes.SYYDiagnoses.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetSYYDiagnosisDefByIdQuery(id),ct));
 /// <summary>Tanı Türü / Dilim / bağlı kayıt seçeneklerini SYY tanı ekranının kendi yetkisiyle getirir.</summary>
 [HttpGet("options")]
 [HasPermission(PermissionCodes.SYYDiagnoses.View, PermissionCodes.SYYDiagnoses.Query, PermissionCodes.SYYDiagnoses.Create, PermissionCodes.SYYDiagnoses.Update, PermissionCodes.SYYDiagnoses.Delete)]
 public async Task<IActionResult> Options([FromQuery] Guid? currentId,[FromQuery] int? currentType,
 [FromQuery] int? currentSlice,[FromQuery] Guid? currentParent,CancellationToken ct)=>
 Ok(await sender.Send(new GetSYYDiagnosisOptionsQuery(currentId,currentType,currentSlice,currentParent),ct));
 [HttpGet("export")]
 [HasPermission(PermissionCodes.SYYDiagnoses.Query)]
 public async Task<IActionResult> Export([FromQuery] GetSYYDiagnosisDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with{PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreleyerek en fazla 5000 kayıt indirin."});
   var rows=new List<SYYDiagnosisDefResponse>(first.Items);
   for(var page=2;page<=(int)Math.Ceiling(first.TotalCount/100d);page++)
      rows.AddRange((await sender.Send(q with{PageNumber=page,PageSize=100},ct)).Items);
   ExcelColumn<SYYDiagnosisDefResponse>[] columns={new("Kod",x=>x.Code,Width:17),new("Ad",x=>x.Name,Width:42),new("Tanı Türü",x=>x.DiagnosisTypeName,Width:26),new("Bağlı Kayıt",x=>x.AttachedDiagnosisName,Width:36),new("Seçilebilir",x=>x.Eligible==null?"Belirsiz":x.Eligible.Value?"Evet":"Hayır"),new("Dilim",x=>x.SliceName,Width:18),new("Geçerli Başlangıç",x=>x.ValidFrom,NumberFormat:"dd.MM.yyyy HH:mm"),new("Geçerli Bitiş",x=>x.ValidTo,NumberFormat:"dd.MM.yyyy HH:mm"),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(rows,columns,"SYY Tanı Listesi (M/D/F)"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "syy-diagnoses_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 [HttpPost]
 [HasPermission(PermissionCodes.SYYDiagnoses.Create)]
 public async Task<IActionResult> Create([FromBody] CreateSYYDiagnosisDefCommand r,CancellationToken ct)
 {
   var item=await sender.Send(r,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnoses.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateSYYDiagnosisDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.SYYDiagnoses.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusSYYDiagnosisDefCommand r,CancellationToken ct)
 {
  if(id!=r.Id)return BadRequest("Kimlik eşleşmedi.");
  return Ok(await sender.Send(r,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.SYYDiagnoses.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteSYYDiagnosisDefCommand(id,version),ct);
   return NoContent();
 }
}
