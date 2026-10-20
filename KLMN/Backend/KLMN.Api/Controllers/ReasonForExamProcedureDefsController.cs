using KLMN.Api.Authorization;
using KLMN.Api.Exports;
using KLMN.Domain.Constants;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.CreateReasonForExamProcedureDef;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.UpdateReasonForExamProcedureDef;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.ChangeStatusReasonForExamProcedureDef;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.DeleteReasonForExamProcedureDef;
using KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefs;
using KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefById;
using KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefOptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace KLMN.Api.Controllers;
/// <summary>Gereken Tetkikler CRUD, Excel, permission ve xmin API uçları.</summary>
[ApiController]
[Route("api/reason-exam-procedures")]
public sealed class ReasonForExamProcedureDefsController(ISender sender,IExcelExportService excel):ControllerBase
{
 /// <summary>Backend arama/sayfalama; modülün tek bir izni bile yeterlidir.</summary>
 [HttpGet]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.View, PermissionCodes.ReasonForExamProcedureDefs.Query, PermissionCodes.ReasonForExamProcedureDefs.Create, PermissionCodes.ReasonForExamProcedureDefs.Update, PermissionCodes.ReasonForExamProcedureDefs.Delete)]
 public async Task<IActionResult> List([FromQuery] GetReasonForExamProcedureDefsQuery q,CancellationToken ct)=>Ok(await sender.Send(q,ct));
 
 /// <summary>Yalnızca Query izni ile en fazla 5000 filtrelenmiş kayıt dışa aktarılır.</summary>
 [HttpGet("export")]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.Query)]
 public async Task<IActionResult> Export([FromQuery] GetReasonForExamProcedureDefsQuery q,CancellationToken ct)
 {
   var first=await sender.Send(q with {PageNumber=1,PageSize=100},ct);
   if(first.TotalCount>5000)return BadRequest(new ProblemDetails{Status=400,Title="Excel aktarım sınırı aşıldı.",Detail="Filtreyi daraltınız (en fazla 5000)."});
   var items=new List<ReasonForExamProcedureDefResponse>(first.Items);
   for(var i=2;i<=(int)Math.Ceiling(first.TotalCount/100d);i++)
   {
       var part=await sender.Send(q with {PageNumber=i,PageSize=100},ct);
       items.AddRange(part.Items);
   }
   var cols=new ExcelColumn<ReasonForExamProcedureDefResponse>[]{new("Muayene Sebebi",x=>x.ReasonDescription,Width:40),new("Tetkik",x=>x.ReferenceName,Width:40),new("Başlangıç Yaşı",x=>x.StartingAge),new("Bitiş Yaşı",x=>x.EndingAge),new("Cinsiyet Kodu",x=>x.Gender),new("Durum",x=>x.IsActive?"Aktif":"Pasif")};
   return File(excel.Export(items,cols,"Gereken Tetkikler"),
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
     "reason-exam-procedures_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmm")+".xlsx");
 }
 /// <summary>Dropdown seçimleri bu ekranın kendi izniyle döner.</summary>
 [HttpGet("options")]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.View, PermissionCodes.ReasonForExamProcedureDefs.Query, PermissionCodes.ReasonForExamProcedureDefs.Create, PermissionCodes.ReasonForExamProcedureDefs.Update)]
 public async Task<IActionResult> Options([FromQuery] Guid? selectedReasonId,[FromQuery] Guid? selectedReferenceId,CancellationToken ct)=>
 Ok(await sender.Send(new GetReasonForExamProcedureDefOptionsQuery(selectedReasonId,selectedReferenceId),ct));
 [HttpGet("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.View, PermissionCodes.ReasonForExamProcedureDefs.Query, PermissionCodes.ReasonForExamProcedureDefs.Create, PermissionCodes.ReasonForExamProcedureDefs.Update, PermissionCodes.ReasonForExamProcedureDefs.Delete)]
 public async Task<IActionResult> ById(Guid id,CancellationToken ct)=>Ok(await sender.Send(new GetReasonForExamProcedureDefByIdQuery(id),ct));
 [HttpPost]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.Create)]
 public async Task<IActionResult> Create([FromBody] CreateReasonForExamProcedureDefCommand q,CancellationToken ct)
 {
   var item=await sender.Send(q,ct);
   return CreatedAtAction(nameof(ById),new{id=item.Id},item);
 }
 [HttpPut("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.Update)]
 public async Task<IActionResult> Update(Guid id,[FromBody] UpdateReasonForExamProcedureDefCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpPatch("{id:guid}/status")]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.Update)]
 public async Task<IActionResult> Status(Guid id,[FromBody] ChangeStatusReasonForExamProcedureDefCommand q,CancellationToken ct)
 {
   if(id!=q.Id)return BadRequest("Kayıt kimlikleri uyuşmuyor.");
   return Ok(await sender.Send(q,ct));
 }
 [HttpDelete("{id:guid}")]
 [HasPermission(PermissionCodes.ReasonForExamProcedureDefs.Delete)]
 public async Task<IActionResult> Delete(Guid id,[FromQuery] uint version,CancellationToken ct)
 {
   await sender.Send(new DeleteReasonForExamProcedureDefCommand(id,version),ct);
   return NoContent();
 }
}
