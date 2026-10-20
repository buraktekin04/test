using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ReasonForExaminations.Responses;
using KLMN.Application.ReasonForExaminations.Commands.CreateReasonForExaminationDef;
using KLMN.Application.ReasonForExaminations.Commands.UpdateReasonForExaminationDef;
using KLMN.Application.ReasonForExaminations.Commands.ChangeStatusReasonForExaminationDef;
using KLMN.Application.ReasonForExaminations.Commands.DeleteReasonForExaminationDef;
using KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefs;
using KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefById;

using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Muayene Sebebi CRUD, Excel, permission ve xmin API uçları.</summary>
[ApiController]
[Route("api/reasons-for-examination")]
public sealed class ReasonsForExaminationController(ISender sender,IExcelExportService excel):ControllerBase
{
 /// <summary>Backend arama/sayfalama; modülün tek bir izni bile yeterlidir.</summary>
 [HttpGet]
 [HasPermission(PermissionCodes.ReasonsForExaminations.View, PermissionCodes.ReasonsForExaminations.Query, PermissionCodes.ReasonsForExaminations.Create, PermissionCodes.ReasonsForExaminations.Update, PermissionCodes.ReasonsForExaminations.Delete)]
 public async Task<IActionResult> List([FromQuery] GetReasonForExaminationDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 
 /// <summary>Yalnızca Query izni ile en fazla 5000 filtrelenmiş kayıt dışa aktarılır.</summary>
 [HttpGet("export")]
 [HasPermission(PermissionCodes.ReasonsForExaminations.Query)]
 public async Task<IActionResult> Export([FromQuery] GetReasonForExaminationDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with {PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreyi daraltınız (en fazla 5000)."});
   var items=new List<ReasonForExaminationDefResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
   {
       var part=await sender.Send(q with {PageNumber=i,PageSize=100},ct);
       items.AddRange(part.Items);
   }
   var cols=new ExcelColumn<ReasonForExaminationDefResponse>[]{new("Kod",x=>x.Code,Width:20),new("Açıklama",x=>x.Description,Width:50),new("Sevk Gerekir",x=>x.DispatchRequired?"Evet":"Hayır"),new("Sınıflandırma Muayenesi",x=>x.ClassificationExamination?"Evet":"Hayır"),new("İtiraz Edilebilir",x=>x.Objectionable?"Evet":"Hayır"),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(items,cols,"Muayene Sebebi"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "reasons-for-examination_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonsForExaminations.View, PermissionCodes.ReasonsForExaminations.Query, PermissionCodes.ReasonsForExaminations.Create, PermissionCodes.ReasonsForExaminations.Update, PermissionCodes.ReasonsForExaminations.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetReasonForExaminationDefByIdQuery(id),ct));
 [HttpPost]
 [HasPermission(PermissionCodes.ReasonsForExaminations.Create)]
 public async Task<IActionResult> Create([FromBody] CreateReasonForExaminationDefCommand q,CancellationToken ct)
 {
   var item=await sender.Send(q,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonsForExaminations.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateReasonForExaminationDefCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.ReasonsForExaminations.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusReasonForExaminationDefCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonsForExaminations.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteReasonForExaminationDefCommand(id,version),ct);
   return NoContent();
 }
}
