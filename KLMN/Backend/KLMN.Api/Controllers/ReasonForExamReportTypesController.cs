using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ReasonForExamReportTypes.Responses;
using KLMN.Application.ReasonForExamReportTypes.Commands.CreateReasonForExamReportType;
using KLMN.Application.ReasonForExamReportTypes.Commands.UpdateReasonForExamReportType;
using KLMN.Application.ReasonForExamReportTypes.Commands.ChangeStatusReasonForExamReportType;
using KLMN.Application.ReasonForExamReportTypes.Commands.DeleteReasonForExamReportType;
using KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypes;
using KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypeById;
using KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypeOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Muayene Sebebi Rapor Türleri CRUD, Excel, permission ve xmin API uçları.</summary>
[ApiController]
[Route("api/reason-exam-report-types")]
public sealed class ReasonForExamReportTypesController(ISender sender,IExcelExportService excel):ControllerBase
{
 /// <summary>Backend arama/sayfalama; modülün tek bir izni bile yeterlidir.</summary>
 [HttpGet]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.View, PermissionCodes.ReasonForExamReportTypes.Query, PermissionCodes.ReasonForExamReportTypes.Create, PermissionCodes.ReasonForExamReportTypes.Update, PermissionCodes.ReasonForExamReportTypes.Delete)]
 public async Task<IActionResult> List([FromQuery] GetReasonForExamReportTypesQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 
 /// <summary>Yalnızca Query izni ile en fazla 5000 filtrelenmiş kayıt dışa aktarılır.</summary>
 [HttpGet("export")]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.Query)]
 public async Task<IActionResult> Export([FromQuery] GetReasonForExamReportTypesQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with {PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreyi daraltınız (en fazla 5000)."});
   var items=new List<ReasonForExamReportTypeResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
   {
       var part=await sender.Send(q with {PageNumber=i,PageSize=100},ct);
       items.AddRange(part.Items);
   }
   var cols=new ExcelColumn<ReasonForExamReportTypeResponse>[]{new("Muayene Sebebi",x=>x.ReasonDescription,Width:44),new("Rapor Türü",x=>x.ReferenceName,Width:40),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(items,cols,"Muayene Sebebi Rapor Türleri"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "reason-exam-report-types_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 /// <summary>Dropdown seçimleri bu ekranın kendi izniyle döner.</summary>
 [HttpGet("options")]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.View, PermissionCodes.ReasonForExamReportTypes.Query, PermissionCodes.ReasonForExamReportTypes.Create, PermissionCodes.ReasonForExamReportTypes.Update)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedReasonId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
 Ok(await sender.Send(new GetReasonForExamReportTypeOptionsQuery(selectedReasonId,selectedReferenceId),ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.View, PermissionCodes.ReasonForExamReportTypes.Query, PermissionCodes.ReasonForExamReportTypes.Create, PermissionCodes.ReasonForExamReportTypes.Update, PermissionCodes.ReasonForExamReportTypes.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetReasonForExamReportTypeByIdQuery(id),ct));
 [HttpPost]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.Create)]
 public async Task<IActionResult> Create([FromBody] CreateReasonForExamReportTypeCommand q,CancellationToken ct)
 {
   var item=await sender.Send(q,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateReasonForExamReportTypeCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusReasonForExamReportTypeCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamReportTypes.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteReasonForExamReportTypeCommand(id,version),ct);
   return NoContent();
 }
}
